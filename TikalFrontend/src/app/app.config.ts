import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';
import { csrfHeaderInterceptor } from './core/interceptors/csrf-header.interceptor';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { AccountStore } from './core/stores/account-store/account-store';
import { AuthStore } from './core/stores/auth-store/auth-store';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withViewTransitions()),
    provideHttpClient(withFetch(), withInterceptors([csrfHeaderInterceptor])),
    provideAppInitializer(() => {
      const accountStore = inject(AccountStore);
      const authStore = inject(AuthStore);

      accountStore.loadAccount();
      authStore.loadSession();
    }),
  ],
};
