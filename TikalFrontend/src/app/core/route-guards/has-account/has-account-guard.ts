import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountStore } from '../../stores/account-store/account-store';
import { AuthStore } from '../../stores/auth-store/auth-store';
import { environment } from '../../../../environments/environment';

export const hasAccount: CanActivateFn = (state) => {
  const authStore = inject(AuthStore);

  if (!authStore.isAuthenticated()) {
    window.location.href = `${environment.backend_url}/bff/login?returnUrl=${window.location.origin}/${state.url}`;
    return false;
  }

  const accountStore = inject(AccountStore);

  if (accountStore.hasAccount()) {
    return true;
  }

  const router = inject(Router);

  return router.createUrlTree(['/createAccount'], {
    queryParams: { returnUrl: state.url },
  });
};
