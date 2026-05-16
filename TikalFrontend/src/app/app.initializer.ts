import { inject, provideAppInitializer } from '@angular/core';
import { AuthStore } from './core/stores/auth-store/auth-store';
import { AccountStore } from './core/stores/account-store/account-store';
import { iif, of, switchMap } from 'rxjs';

export const appInitializer = provideAppInitializer(() => {
  const authStore = inject(AuthStore);
  const accountStore = inject(AccountStore);

  return authStore
    .loadSession()
    .pipe(switchMap((result) => iif(() => result.isOk(), accountStore.loadAccount(), of(null))));
});
