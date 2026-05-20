import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountStore } from '../../stores/account-store/account-store';

export const hasNoAccount: CanActivateFn = (state) => {
  const accountStore = inject(AccountStore);

  if (!accountStore.hasAccount()) {
    return true;
  }

  const router = inject(Router);

  const url = state.queryParamMap.get('returnUrl') ?? '/';

  return router.createUrlTree([url]);
};
