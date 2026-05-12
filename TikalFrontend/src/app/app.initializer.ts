import { inject, provideAppInitializer } from '@angular/core';
import { AuthStore } from './core/stores/auth-store/auth-store';

export const appInitializer = provideAppInitializer(() => {
  const authStore = inject(AuthStore);

  return authStore.loadSession();
});
