import { catchError, firstValueFrom, Observable, of, throwError } from 'rxjs';
import { Account, AccountService } from '../../services/account-service/account-service';
import { err, ok, Result } from 'neverthrow';
import { TestBed } from '@angular/core/testing';
import { AccountStore } from './account-store';
import { Conflict, NotFound } from '../../dtos/errors';

const DEFAULT_ACCOUNT: Account = {
  userId: 'userId',
  name: 'name',
};

describe('AccountStore', () => {
  // dependencies
  const successAccountService = {
    getAccount: (): Observable<Account | null> => of(DEFAULT_ACCOUNT),
    createAccount: (): Observable<Result<Account, Conflict>> => of(ok(DEFAULT_ACCOUNT)),
  };

  const notFoundAccountService = {
    getAccount: (): Observable<Account | null> => of(null),
  };

  const conflictAccountService = {
    createAccount: (): Observable<Result<Account, Conflict>> =>
      of(err({ type: 'Conflict' } as const)),
  };

  const throwingAccountService = {
    getAccount: (): Observable<Result<Account, NotFound>> => throwError(err),
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
    await firstValueFrom(store.loadAccount().pipe(catchError(err)));

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

  test('createAccount does not set account if account creation results in a conflict', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: AccountService, useValue: conflictAccountService }],
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
