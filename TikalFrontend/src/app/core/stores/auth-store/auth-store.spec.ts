import { catchError, firstValueFrom, Observable, of, throwError } from 'rxjs';
import { AuthService, Claim, Session } from '../../services/auth-service/auth-service';
import { AuthStore } from './auth-store';
import { TestBed } from '@angular/core/testing';

const DEFAULT_SESSION: Claim[] = [
  { type: 'sub', value: 'userId' },
  { type: 'bff:logout_url', value: 'logout_url' },
];

describe('AuthStore', () => {
  // dependencies
  const successAuthService = {
    getSession: (): Observable<Session | null> => of(DEFAULT_SESSION),
  };

  const unauthorizedAuthService = {
    getSession: (): Observable<Session | null> => of(null),
  };

  const throwingAuthService = {
    getSession: (): Observable<Session | null> => throwError(() => of(null)),
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
    await firstValueFrom(store.loadSession().pipe(catchError(() => of(null))));

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
