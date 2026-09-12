import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { AuthService, Session } from '../../services/auth-service/auth-service';
import { computed, inject } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';

export type AuthState = {
  initializationFailed: boolean;
  session: Session | null;
};

const initalState: AuthState = {
  initializationFailed: false,
  session: null,
};

export const AuthStore = signalStore(
  { providedIn: 'root' },

  withState(initalState),

  withProps(() => ({
    _authService: inject(AuthService),
  })),

  withComputed(({ session }) => ({
    isAuthenticated: computed(() => session() !== null),

    userId: computed(() => session()?.find((claim) => claim.type === 'sub')?.value),

    logoutUrl: computed(
      () =>
        `${environment.backend_url}` +
        session()?.find((claim) => claim.type === 'bff:logout_url')?.value +
        `&returnUrl=${globalThis.location.origin}`,
    ),
  })),

  withMethods((store) => ({
    // this method returns an observable because it needs to run during app initialization
    loadSession(): Observable<Session | null> {
      return store._authService.getSession().pipe(
        tap((result: Session | null) => {
          if (result !== null) {
            patchState(store, { session: result });
          }
        }),
        catchError((error) => {
          patchState(store, { initializationFailed: true });

          return throwError(() => error);
        }),
      );
    },
  })),
);
