import { AngularRenderer, applicationConfig, type Preview } from '@storybook/angular-vite';
import { setCompodocJson } from '@storybook/addon-docs/angular';
import { isDevMode, provideZonelessChangeDetection } from '@angular/core';
import { providePrimeNG } from 'primeng/config';
import { withThemeByClassName } from '@storybook/addon-themes';
import docJson from '../documentation.json';
import TikalTheme from '../src/lib/theme/tikal-theme';
import { provideTransloco } from '@jsverse/transloco';
import { TranslocoHttpLoader } from './transloco-loader';
setCompodocJson(docJson);

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
        providePrimeNG({
          license:
            'eyJpZCI6IjJmYWE2OGNmLTZkYTItNDlmNS1iZWY1LTNhYWUxMTcwZmFiMCIsInByb2R1Y3QiOiJwcmltZXVpIiwidGllciI6ImNvbW11bml0eSIsInR5cGUiOiJkZXYiLCJpYXQiOjE3ODcyOTAwNzEsImV4cCI6MTgxODgyNjA3MX0.c_zo_mjRE48LO8Ivrsc4fE6D33GDJJRgn2HoXuETWhIve_Tzw3rK3yGGg3BF5-T0KBJ-_KO0hicRAvKrY1vKBA',
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
