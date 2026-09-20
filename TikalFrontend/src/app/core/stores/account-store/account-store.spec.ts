import { catchError, firstValueFrom, Observable, of, throwError } from 'rxjs';
import {
  Account,
  AccountExists,
  AccountService,
} from '../../services/account-service/account-service';
import { TestBed } from '@angular/core/testing';
import { AccountStore } from './account-store';
import { err, ok, Result } from 'neverthrow';

const DEFAULT_ACCOUNT: Account = {
  userId: 'userId',
  name: 'name',
};

describe('AccountStore', () => {
  // dependencies
  const successAccountService = {
    getAccount: (): Observable<Account | null> => of(DEFAULT_ACCOUNT),
    createAccount: (): Observable<Result<Account, AccountExists>> => of(ok(DEFAULT_ACCOUNT)),
  };

  const notFoundAccountService = {
    getAccount: (): Observable<Account | null> => of(null),
  };

  const throwingAccountService = {
    getAccount: (): Observable<Account | null> => throwError(() => null),
  };

  const accountExistsAccountService = {
    createAccount: (name: string): Observable<Result<Account, AccountExists>> =>
      of(err({ name: name })),
  };

  test('loadAccount does not set account if account can not be found', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AccountService, useValue: notFoundAccountService }],
    });

    const store = TestBed.inject(AccountStore);

    // when
    await firstValueFrom(store.loadAccount());

    // then
    expect(store.hasAccount()).toBeFalsy();
    expect(store.account()).toEqual(null);
  });

  test('loadAccount sets initializationFailed if account retrieval throws', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AccountService, useValue: throwingAccountService }],
    });

    const store = TestBed.inject(AccountStore);

    // when
    await firstValueFrom(store.loadAccount().pipe(catchError(() => of(null))));

    // then
    expect(store.initializationFailed()).toBeTruthy();
  });

  test('loadAccount sets account if account retrieval is successful', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AccountService, useValue: successAccountService }],
    });

    const store = TestBed.inject(AccountStore);

    // when
    await firstValueFrom(store.loadAccount());

    // then
    expect(store.hasAccount()).toBeTruthy();
    expect(store.account()).toEqual(DEFAULT_ACCOUNT);
  });

  test('createAccount does not set account if account already exists', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AccountService, useValue: accountExistsAccountService }],
    });

    const store = TestBed.inject(AccountStore);

    // when
    await store.createAccount(DEFAULT_ACCOUNT.name);

    // then
    expect(store.hasAccount()).toBeFalsy();
    expect(store.account()).toEqual(null);
  });

  test('createAccount sets account if account creation succeeds', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AccountService, useValue: successAccountService }],
    });

    const store = TestBed.inject(AccountStore);

    // when
    await store.createAccount(DEFAULT_ACCOUNT.name);

    // then
    expect(store.hasAccount()).toBeTruthy();
    expect(store.account()).toEqual(DEFAULT_ACCOUNT);
  });
});
