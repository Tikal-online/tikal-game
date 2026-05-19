import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountStore } from '../../stores/account-store/account-store';

export const hasAccount: CanActivateFn = (state) => {
  const accountStore = inject(AccountStore);

  if (accountStore.hasAccount()) {
    return true;
  }

  const router = inject(Router);

  return router.createUrlTree(['/createAccount'], {
    queryParams: { returnUrl: state.url },
  });
};
