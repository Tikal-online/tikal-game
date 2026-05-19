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
import { catchError, Observable, tap, throwError } from 'rxjs';
import { Result } from 'neverthrow';
import { environment } from '../../../../environments/environment';

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
    // this method returns an observable instead of being an rxMethod so it can run during app initialization
    loadSession(): Observable<Result<Session, Unauthorized>> {
      patchState(store, { status: 'loading' });

      return store._authService.getSession().pipe(
        tap((result: Result<Session, Unauthorized>) => {
          if (result.isOk()) {
            patchState(store, { session: result.value, status: 'fulfilled' });
          } else {
            patchState(store, { status: 'unauthorized' });
          }
        }),
        catchError((error) => {
          patchState(store, { status: 'serverError' });

          return throwError(() => error);
        }),
      );
    },
  })),
);
