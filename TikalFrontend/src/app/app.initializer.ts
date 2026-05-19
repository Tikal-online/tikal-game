import { inject, provideAppInitializer } from '@angular/core';
import { AuthStore } from './core/stores/auth-store/auth-store';
import { AccountStore } from './core/stores/account-store/account-store';
import { catchError, iif, of, switchMap } from 'rxjs';

export const appInitializer = provideAppInitializer(() => {
  const authStore = inject(AuthStore);
  const accountStore = inject(AccountStore);

  return authStore.loadSession().pipe(
    // only attempt to load the game account if we have an active session and are authenticated
    switchMap((result) => iif(() => result.isOk(), accountStore.loadAccount(), of(null))),
    catchError(() => of(null)),
  );
});
