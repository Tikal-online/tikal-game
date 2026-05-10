import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { AuthService, Session } from '../../services/auth-service/auth-service';
import { computed, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { exhaustMap, pipe, tap } from 'rxjs';

type AuthState = {
  session: Session | null;
};

const initalState: AuthState = {
  session: null,
};

export const AuthStore = signalStore(
  withState(initalState),
  withComputed(({ session }) => ({
    isAuthenticated: computed(() => session() !== null),
  })),
  withMethods((store, authService = inject(AuthService)) => ({
    loadSession: rxMethod<void>(
      pipe(
        exhaustMap(() => {
          return authService.GetSession().pipe(
            tap((result) => {
              patchState(store, { session: result.isOk() ? result.value : null });
            }),
          );
        }),
      ),
    ),
  })),
);
