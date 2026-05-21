import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthStore } from '../../stores/auth-store/auth-store';
import { environment } from '../../../../environments/environment';

export const isAuthenticated: CanActivateFn = (state) => {
  const authStore = inject(AuthStore);

  if (authStore.isAuthenticated()) {
    return true;
  }

  globalThis.location.href = `${environment.backend_url}/bff/login?returnUrl=${globalThis.location.origin}/${state.url}`;
  return false;
};
