import { HttpClient } from '@angular/common/http';
import { Injectable, Signal, computed, inject } from '@angular/core';
import { catchError, shareReplay, Observable, defer } from 'rxjs';
import { of } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { environment } from '../../environments/environment';

const ANONYMOUS: Session = null;
const CACHE_SIZE = 1;

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private session$: Observable<Session> | null = null;

  // Create a signal from the getSession() observable.
  // Maybe in the future this can be a Resource once the api stabilizes.
  readonly session: Signal<Session> = toSignal(
    defer(() => this.getSession()), // Defer the getSession call
    { initialValue: ANONYMOUS },
  );
  // Derived signals using computed that automatically update.
  readonly isAuthenticated = computed(() => this.session() !== null);
  readonly isAnonymous = computed(() => this.session() === null);
  readonly logoutUrl = computed(() => {
    const session = this.session();
    return session
      ? `${environment.backend_url}` +
          session.find((c) => c.type === 'bff:logout_url')?.value +
          `&returnUrl=${window.location.origin}` || null
      : null;
  });

  getSession(ignoreCache = false): Observable<Session> {
    if (!this.session$ || ignoreCache) {
      this.session$ = this.http.get<Session>(`${environment.backend_url}/bff/user`).pipe(
        catchError(() => of(ANONYMOUS)),
        shareReplay(CACHE_SIZE),
      );
    }
    return this.session$;
  }
}

export type Claim = {
  type: string;
  value: string;
};
export type Session = Claim[] | null;
