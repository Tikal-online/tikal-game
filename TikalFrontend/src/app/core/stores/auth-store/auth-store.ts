import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { AuthService, Session, Unauthorized } from '../../services/auth-service/auth-service';
import { computed, inject } from '@angular/core';
import { catchError, pipe, switchMap, tap, throwError } from 'rxjs';
import { Result } from 'neverthrow';
import { environment } from '../../../../environments/environment';
import { rxMethod } from '@ngrx/signals/rxjs-interop';

export type AuthStatus = 'idle' | 'loading' | 'fulfilled' | 'unauthorized' | 'serverError';

export type AuthState = {
  status: AuthStatus;
  session: Session | null;
};

const initalState: AuthState = {
  status: 'idle',
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
    logoutUrl: computed(
      () =>
        `${environment.backend_url}` +
        session()?.find((claim) => claim.type === 'bff:logout_url')?.value +
        `&returnUrl=${window.location.origin}`,
    ),
  })),

  withMethods((store) => ({
    loadSession: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { status: 'loading' })),
        switchMap(() => {
          return store._authService.getSession().pipe(
            tap((result: Result<Session, Unauthorized>) => {
              if (result.isOk()) {
                patchState(store, { session: result.value, status: 'fulfilled' });
              } else {
                patchState(store, { session: null, status: 'unauthorized' });
              }
            }),
            catchError((error) => {
              patchState(store, { session: null, status: 'serverError' });

              return throwError(() => error);
            }),
          );
        }),
      ),
    ),
  })),
);
