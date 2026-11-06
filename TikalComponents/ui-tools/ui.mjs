#!/usr/bin/env node
// Storybook inspection tool for the locally running (Aspire) Tikal component library.
// Lets a developer — or an AI agent — screenshot stories in light and dark mode and review them at a glance.
//
// Usage:
//   node ui-tools/ui.mjs list [filter]                 list story ids (optionally filtered by substring)
//   node ui-tools/ui.mjs shot [filter] [options]       screenshot matching stories + build an overview sheet
//
// Options:
//   --theme light|dark|all    (default: all)
//   --width <px>              viewport width (default: 900)
//
// Env: UI_STORYBOOK_URL (default https://localhost:6006)

import { chromium } from 'playwright';
import { mkdir, readFile, writeFile } from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const OUT_DIR = path.join(ROOT, '.screenshots');
const BASE_URL = process.env.UI_STORYBOOK_URL ?? 'https://localhost:6006';
const BACKGROUND = { light: '#f5f3ed', dark: '#151c16' };

function parseArgs(argv) {
  const [command, ...rest] = argv;
  const opts = { theme: 'all', width: '900', filter: '' };
  for (let i = 0; i < rest.length; i++) {
    if (rest[i].startsWith('--')) opts[rest[i].slice(2)] = rest[++i];
    else opts.filter = rest[i];
  }
  return { command, opts };
}

async function fetchStories(browser, filter) {
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const response = await context.request.get(`${BASE_URL}/index.json`);
  if (!response.ok())
    throw new Error(
      `could not load ${BASE_URL}/index.json (${response.status()}) — is Storybook running?`,
    );
  const { entries } = await response.json();
  await context.close();
  return Object.values(entries)
    .filter((entry) => entry.type === 'story' && entry.id.includes(filter))
    .map((entry) => entry.id);
}

async function screenshotStory(page, id, theme) {
  const errors = [];
  // "Failed to load resource" console messages carry no URL; failed responses are reported below instead.
  const onConsole = (msg) =>
    msg.type() === 'error' &&
    !msg.text().startsWith('Failed to load resource') &&
    errors.push(msg.text());
  const onPageError = (err) => errors.push(err.message);
  const onResponse = (res) => res.status() >= 400 && errors.push(`${res.status()} ${res.url()}`);
  page.on('console', onConsole);
  page.on('pageerror', onPageError);
  page.on('response', onResponse);

  await page.goto(`${BASE_URL}/iframe.html?id=${id}&viewMode=story&globals=theme:${theme}`);
  await page.waitForSelector('#storybook-root > *', { timeout: 20_000 });
  await page.waitForLoadState('networkidle');
  const file = path.join(OUT_DIR, `${id}-${theme}.png`);
  await page.locator('#storybook-root').screenshot({ path: file });

  page.off('console', onConsole);
  page.off('pageerror', onPageError);
  page.off('response', onResponse);
  for (const error of errors) console.log(`  [${id} ${theme}] ${error}`);
  return file;
}

async function buildSheet(browser, rows, themes) {
  const cell = async (file, theme) =>
    `<div style="padding:12px;background:${BACKGROUND[theme]}"><img src="data:image/png;base64,${(await readFile(file)).toString('base64')}"></div>`;
  let html = `<body style="margin:0;font:12px monospace;display:grid;grid-template-columns:repeat(${themes.length},1fr)">`;
  for (const row of rows) {
    for (const theme of themes) {
      html += `<div style="padding:6px 12px 0;background:${BACKGROUND[theme]};color:#888">${row.id} · ${theme}</div>`;
    }
    for (const theme of themes) html += await cell(row.files[theme], theme);
  }
  html += '</body>';

  const page = await browser.newPage({ viewport: { width: 760 * themes.length, height: 400 } });
  await page.setContent(html);
  await page.waitForLoadState('load');
  const file = path.join(OUT_DIR, 'sheet.png');
  await page.screenshot({ path: file, fullPage: true });
  await writeFile(path.join(OUT_DIR, 'sheet.html'), html);
  await page.close();
  return file;
}

async function main() {
  const { command, opts } = parseArgs(process.argv.slice(2));
  if (!['list', 'shot'].includes(command)) {
    throw new Error(
      'usage: ui.mjs list [filter] | shot [filter] [--theme light|dark|all] [--width px]',
    );
  }

  const browser = await chromium.launch();
  try {
    const ids = await fetchStories(browser, opts.filter);
    if (ids.length === 0) throw new Error(`no stories match "${opts.filter}"`);
    if (command === 'list') return ids.forEach((id) => console.log(id));

    const themes = opts.theme === 'all' ? ['light', 'dark'] : [opts.theme];
    await mkdir(OUT_DIR, { recursive: true });
    const page = await browser.newPage({
      ignoreHTTPSErrors: true,
      viewport: { width: Number(opts.width), height: 600 },
    });
    const rows = [];
    for (const id of ids) {
      const files = {};
      for (const theme of themes) files[theme] = await screenshotStory(page, id, theme);
      rows.push({ id, files });
      console.log(`screenshot: ${id} (${themes.join(', ')})`);
    }
    console.log(`overview: ${await buildSheet(browser, rows, themes)}`);
  } finally {
    await browser.close();
  }
}

main().catch((err) => {
  console.error(err.message);
  process.exit(1);
});
