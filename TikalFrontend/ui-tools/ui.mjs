#!/usr/bin/env node
// UI inspection tool for the locally running (Aspire) Tikal frontend.
// Lets a developer — or an AI agent — log in, take screenshots and drive interactions headlessly.
//
// Usage:
//   node ui-tools/ui.mjs login                                   force a fresh login and store the session
//   node ui-tools/ui.mjs shot <route> [options]                  screenshot a route
//   node ui-tools/ui.mjs run <steps.json> [options]              run a scripted interaction (see README.md)
//
// Options:
//   --viewport desktop|tablet|mobile|all   (default: desktop)
//   --theme light|dark|all                 (default: light)
//   --full                                 capture the full scrollable page
//   --name <prefix>                        screenshot file name prefix
//
// Env: UI_BASE_URL (default https://localhost:4200), UI_USER (default test), UI_PASS (default Pass123$)

import { chromium } from '@playwright/test';
import { mkdir, readFile, access } from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const AUTH_FILE = path.join(ROOT, '.auth', 'state.json');
const OUT_DIR = path.join(ROOT, '.screenshots');

const BASE_URL = process.env.UI_BASE_URL ?? 'https://localhost:4200';
const USER = process.env.UI_USER ?? 'test';
const PASS = process.env.UI_PASS ?? 'Pass123$';

const VIEWPORTS = {
  desktop: { width: 1440, height: 900 },
  tablet: { width: 820, height: 1180 },
  mobile: { width: 390, height: 844 },
};

function parseArgs(argv) {
  const [command, target, ...rest] = argv;
  const opts = { viewport: 'desktop', theme: 'light', full: false, name: undefined };
  for (let i = 0; i < rest.length; i++) {
    const arg = rest[i];
    if (arg === '--full') opts.full = true;
    else if (arg.startsWith('--')) opts[arg.slice(2)] = rest[++i];
  }
  return { command, target, opts };
}

const expand = (value, all) => (value === 'all' ? all : [value]);

async function exists(file) {
  try {
    await access(file);
    return true;
  } catch {
    return false;
  }
}

async function login(browser) {
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();
  // Visiting a guarded route redirects to BFF -> IdentityServer login page.
  await page.goto(`${BASE_URL}/Lobbies`);
  await page.waitForSelector('input[name="Input.Username"]', { timeout: 30_000 });
  await page.fill('input[name="Input.Username"]', USER);
  await page.fill('input[name="Input.Password"]', PASS);
  await page.click('button[name="Input.Button"][value="login"]');
  await page.waitForURL((url) => url.href.startsWith(BASE_URL), { timeout: 30_000 });
  await page.waitForLoadState('networkidle');
  await mkdir(path.dirname(AUTH_FILE), { recursive: true });
  await context.storageState({ path: AUTH_FILE });
  console.log(`logged in as "${USER}", landed on ${page.url()}`);
  await context.close();
}

async function newPage(browser, viewport, theme) {
  if (!(await exists(AUTH_FILE))) await login(browser);
  const context = await browser.newContext({
    ignoreHTTPSErrors: true,
    storageState: AUTH_FILE,
    viewport: VIEWPORTS[viewport],
    colorScheme: theme,
    deviceScaleFactor: 1,
  });
  const page = await context.newPage();
  page.on('console', (msg) => {
    if (msg.type() === 'error' || msg.type() === 'warning')
      console.log(`[console.${msg.type()}] ${msg.text()}`);
  });
  page.on('pageerror', (err) => console.log(`[pageerror] ${err.message}`));
  return { context, page };
}

// Navigates and transparently re-logs in when the stored session has expired.
async function goto(browser, ctx, route) {
  const url = new URL(route, BASE_URL).href;
  await ctx.page.goto(url);
  await ctx.page.waitForLoadState('networkidle');
  if (!ctx.page.url().startsWith(BASE_URL)) {
    console.log('session expired, logging in again');
    await ctx.context.close();
    await login(browser);
    Object.assign(ctx, await newPage(browser, ctx.viewport, ctx.theme));
    await ctx.page.goto(url);
    await ctx.page.waitForLoadState('networkidle');
  }
}

let shotCounter = 0;
async function screenshot(page, { name, full, viewport, theme }) {
  await mkdir(OUT_DIR, { recursive: true });
  const file = path.join(OUT_DIR, `${name ?? `shot-${++shotCounter}`}-${viewport}-${theme}.png`);
  await page.screenshot({ path: file, fullPage: full });
  console.log(`screenshot: ${file}`);
}

async function runSteps(browser, ctx, steps, opts) {
  for (const step of steps) {
    const { page } = ctx;
    const [action] = Object.keys(step);
    const arg = step[action];
    console.log(`> ${action} ${typeof arg === 'string' ? arg : JSON.stringify(arg)}`);
    switch (action) {
      case 'goto':
        await goto(browser, ctx, arg);
        break;
      case 'click':
        await page.locator(arg).first().click();
        break;
      case 'fill':
        await page.locator(arg.selector).first().fill(arg.value);
        break;
      case 'press':
        await page.keyboard.press(arg);
        break;
      case 'hover':
        await page.locator(arg).first().hover();
        break;
      case 'waitFor':
        await page.locator(arg).first().waitFor();
        break;
      case 'wait':
        await page.waitForTimeout(arg);
        break;
      case 'shot':
        await screenshot(page, {
          ...opts,
          viewport: ctx.viewport,
          theme: ctx.theme,
          name: arg || undefined,
        });
        break;
      case 'text':
        console.log(
          (
            await page
              .locator(arg ?? 'body')
              .first()
              .innerText()
          ).trim(),
        );
        break;
      default:
        throw new Error(`unknown step "${action}"`);
    }
  }
}

async function main() {
  const { command, target, opts } = parseArgs(process.argv.slice(2));
  const browser = await chromium.launch();
  try {
    if (command === 'login') return await login(browser);

    let steps;
    if (command === 'shot' && target) steps = [{ goto: target }, { shot: opts.name ?? '' }];
    else if (command === 'run' && target) steps = JSON.parse(await readFile(target, 'utf8'));
    else
      throw new Error(
        'usage: ui.mjs login | shot <route> | run <steps.json>  [--viewport ..] [--theme ..] [--full] [--name ..]',
      );

    for (const viewport of expand(opts.viewport, Object.keys(VIEWPORTS))) {
      for (const theme of expand(opts.theme, ['light', 'dark'])) {
        const ctx = { viewport, theme, ...(await newPage(browser, viewport, theme)) };
        await runSteps(browser, ctx, steps, opts);
        await ctx.context.close();
      }
    }
  } finally {
    await browser.close();
  }
}

main().catch((err) => {
  console.error(err.message);
  process.exit(1);
});
