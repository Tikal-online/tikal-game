import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { AuthService, Session, Unauthorized } from '../../services/auth-service/auth-service';
import { computed, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Result } from 'neverthrow';
import { environment } from '../../../../environments/environment';

type AuthState = {
  session: Session | null;
};

const initalState: AuthState = {
  session: null,
};

export const AuthStore = signalStore(
  { providedIn: 'root' },
  withState(initalState),
  withComputed(({ session }) => ({
    isAuthenticated: computed(() => session() !== null),
    logoutUrl: computed(
      () =>
        `${environment.backend_url}` +
        session()?.find((claim) => claim.type === 'bff:logout_url')?.value +
        `&returnUrl=${window.location.origin}`,
    ),
  })),
  withMethods((store, authService = inject(AuthService)) => ({
    loadSession(): Observable<Result<Session, Unauthorized>> {
      return authService.getSession().pipe(
        tap((result) => {
          patchState(store, { session: result.isOk() ? result.value : null });
        }),
      );
    },
  })),
);
