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
};
export default config;
