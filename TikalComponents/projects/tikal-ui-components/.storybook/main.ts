import path from 'node:path';
import { fileURLToPath } from 'node:url';
import type { StorybookConfig } from '@storybook/angular-vite';

const storybookDir = path.dirname(fileURLToPath(import.meta.url));

const config: StorybookConfig = {
  stories: ['../src/**/*.stories.@(js|jsx|mjs|ts|tsx)'],
  addons: ['@storybook/addon-docs', '@storybook/addon-themes', '@storybook/addon-vitest'],
  features: {
    sidebarOnboardingChecklist: false,
  },
  framework: {
    name: '@storybook/angular-vite',
    options: {
      tsconfig: path.join(storybookDir, 'tsconfig.json'),
      compodoc: true,
      compodocArgs: ['-e', 'json', '-d', 'projects/tikal-ui-components'],
    },
  },
  async viteFinal(viteConfig) {
    const { mergeConfig, searchForWorkspaceRoot } = await import('vite');

    // The library has its own package.json, so Vite restricts file serving to the library folder
    // and refuses files from the workspace node_modules (e.g. Storybook's mocker runtime -> 404).
    // Allow the whole workspace, as recommended in https://vite.dev/config/server-options#server-fs-allow
    return mergeConfig(viteConfig, {
      server: { fs: { allow: [searchForWorkspaceRoot(process.cwd())] } },
    });
  },
};
export default config;
