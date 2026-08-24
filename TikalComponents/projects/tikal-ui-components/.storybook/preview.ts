import { AngularRenderer, applicationConfig, type Preview } from '@storybook/angular-vite';
import { setCompodocJson } from '@storybook/addon-docs/angular';
import { provideZonelessChangeDetection } from '@angular/core';
import { providePrimeNG } from 'primeng/config';
import { withThemeByClassName } from '@storybook/addon-themes';
import docJson from '../documentation.json';
import TikalTheme from '../src/lib/theme/tikal-theme';
setCompodocJson(docJson);

if (typeof window !== 'undefined') {
  // Prevent play-function interactions (userEvent.click/type call .focus()
  // internally, which scrolls by default) from moving the Docs page.
  console.log('[preview.ts] patching focus');
  const originalFocus = HTMLElement.prototype.focus;
  HTMLElement.prototype.focus = function (options): void {
    return originalFocus.call(this, { ...options, preventScroll: true });
  };
}

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
    docs: {
      story: {
        autoplay: true,
      },
    },
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
