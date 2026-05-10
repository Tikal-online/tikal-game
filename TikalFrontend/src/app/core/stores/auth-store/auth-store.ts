import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { AuthService, Session, Unauthorized } from '../../services/auth-service/auth-service';
import { computed, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Result } from 'neverthrow';

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
  })),
  withMethods((store, authService = inject(AuthService)) => ({
    loadSession(): Observable<Result<Session, Unauthorized>> {
      return authService.GetSession().pipe(
        tap((result) => {
          patchState(store, { session: result.isOk() ? result.value : null });
        }),
      );
    },
  })),
);
