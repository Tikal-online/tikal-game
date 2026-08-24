import { AngularRenderer, applicationConfig, type Preview } from '@storybook/angular-vite';
import { setCompodocJson } from '@storybook/addon-docs/angular';
import { provideZonelessChangeDetection } from '@angular/core';
import { providePrimeNG } from 'primeng/config';
import { withThemeByClassName } from '@storybook/addon-themes';
import docJson from '../documentation.json';
import TikalTheme from '../src/lib/theme/tikal-theme';
setCompodocJson(docJson);

const preview: Preview = {
  decorators: [
    applicationConfig({
      providers: [
        provideZonelessChangeDetection(),
        providePrimeNG({
          license: import.meta.env['STORYBOOK_PRIMENG_LICENSE'],
          theme: {
            preset: TikalTheme,
            options: {
              darkModeSelector: '.dark',
            },
          },
        }),
      ],
    }),
    withThemeByClassName<AngularRenderer>({
      themes: {
        light: '',
        dark: 'dark',
      },
      defaultTheme: 'light',
    }),
  ],

  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },

    a11y: {
      // 'todo' - show a11y violations in the test UI only
      // 'error' - fail CI on a11y violations
      // 'off' - skip a11y checks entirely
      test: 'todo',
    },
  },
};

export default preview;
