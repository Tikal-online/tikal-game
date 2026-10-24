import { AngularRenderer, applicationConfig, type Preview } from '@storybook/angular-vite';
import { isDevMode, provideZonelessChangeDetection } from '@angular/core';
import { provideOptimus } from '@openng/optimus-ui/config';
import { withThemeByClassName } from '@storybook/addon-themes';
import TikalTheme from '../src/lib/theme/tikal-theme';
import { provideTransloco } from '@jsverse/transloco';
import { TranslocoHttpLoader } from './transloco-loader';

const preview: Preview = {
  decorators: [
    applicationConfig({
      providers: [
        provideTransloco({
          config: {
            availableLangs: ['en'],
            defaultLang: 'en',
            reRenderOnLangChange: true,
            prodMode: !isDevMode(),
          },
          loader: TranslocoHttpLoader,
        }),
        provideZonelessChangeDetection(),
        provideOptimus({
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
