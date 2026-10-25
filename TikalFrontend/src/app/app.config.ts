import { ApplicationConfig, provideBrowserGlobalErrorListeners, isDevMode } from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { appInitializer } from './app.initializer';
import { TranslocoHttpLoader } from './transloco-loader';
import { provideTransloco } from '@jsverse/transloco';
import { baseUrlInterceptor } from './core/interceptors/base-url/base-url.interceptor';
import { csrfHeaderInterceptor } from './core/interceptors/csrf-header/csrf-header.interceptor';
import { providePrimeNG } from 'primeng/config';
import TikalTheme from './core/theme/tikal-theme';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withViewTransitions()),
    provideHttpClient(withInterceptors([csrfHeaderInterceptor, baseUrlInterceptor])),
    appInitializer,
    provideHttpClient(),
    provideTransloco({
      config: {
        availableLangs: ['en'],
        defaultLang: 'en',
        // Remove this option if your application doesn't support changing language in runtime.
        reRenderOnLangChange: true,
        prodMode: !isDevMode(),
      },
      loader: TranslocoHttpLoader,
    }),
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
};
