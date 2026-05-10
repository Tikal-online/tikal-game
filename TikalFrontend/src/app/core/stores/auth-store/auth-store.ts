import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { AuthService, Session } from '../../services/auth-service/auth-service';
import { inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { exhaustMap, pipe, tap } from 'rxjs';

enum AuthStatus {
  Authorized,
  Unauthorized,
}

type AuthState = {
  status: AuthStatus;
  session: Session | null;
};

const initalState: AuthState = {
  status: AuthStatus.Unauthorized,
  session: null,
};

export const AuthStore = signalStore(
  withState(initalState),
  withMethods((store, authService = inject(AuthService)) => ({
    loadSession: rxMethod<void>(
      pipe(
        exhaustMap(() => {
          return authService.GetSession().pipe(
            tap((result) => {
              patchState(
                store,
                result.isOk()
                  ? { status: AuthStatus.Authorized, session: result.value }
                  : { status: AuthStatus.Unauthorized, session: null },
              );
            }),
          );
        }),
      ),
    ),
  })),
);
