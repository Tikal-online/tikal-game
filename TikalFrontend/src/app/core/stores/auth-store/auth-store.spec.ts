import { catchError, firstValueFrom, Observable, of, throwError } from 'rxjs';
import { AuthService, Claim, Session } from '../../services/auth-service/auth-service';
import { err, ok, Result } from 'neverthrow';
import { AuthStore } from './auth-store';
import { TestBed } from '@angular/core/testing';
import { Unauthorized } from '../../dtos/errors';

const DEFAULT_SESSION: Claim[] = [
  { type: 'sub', value: 'userId' },
  { type: 'bff:logout_url', value: 'logout_url' },
];

describe('AuthStore', () => {
  // dependencies
  const successAuthService = {
    getSession: (): Observable<Result<Session, Unauthorized>> => of(ok(DEFAULT_SESSION)),
  };

  const unauthorizedAuthService = {
    getSession: (): Observable<Result<Session, Unauthorized>> =>
      of(err({ type: 'Unauthorized' } as const)),
  };

  const throwingAuthService = {
    getSession: (): Observable<Result<Session, Unauthorized>> => throwError(() => ok()),
  };

  test('loadSession does not set session if session retrieval is unauthorized', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AuthService, useValue: unauthorizedAuthService }],
    });

    const store = TestBed.inject(AuthStore);

    // when
    await firstValueFrom(store.loadSession());

    // then
    expect(store.session()).toEqual(null);
  });

  test('loadSession sets initializationFailed if session retrieval throws', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AuthService, useValue: throwingAuthService }],
    });

    const store = TestBed.inject(AuthStore);

    // when
    await firstValueFrom(store.loadSession().pipe(catchError(err)));

    // then
    expect(store.initializationFailed()).toBeTruthy();
  });

  test('loadSession sets session if session retrieval is successful', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AuthService, useValue: successAuthService }],
    });

    const store = TestBed.inject(AuthStore);

    // when
    await firstValueFrom(store.loadSession());

    // then
    expect(store.session()).toEqual(DEFAULT_SESSION);
  });

  test('userId, logoutUrl and isAuthenticated are set correctly when session is set', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AuthService, useValue: successAuthService }],
    });

    const store = TestBed.inject(AuthStore);

    // when
    await firstValueFrom(store.loadSession());

    // then
    expect(store.isAuthenticated()).toBeTruthy();
    expect(store.userId()).toEqual(DEFAULT_SESSION.find((claim) => claim.type === 'sub')?.value);
    expect(store.logoutUrl()).toContain(
      DEFAULT_SESSION.find((claim) => claim.type === 'bff:logout_url')?.value,
    );
  });
});
