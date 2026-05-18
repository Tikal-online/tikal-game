import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import {
  Account,
  AccountService,
  Conflict,
  NotFound,
} from '../../services/account-service/account-service';
import { computed, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { catchError, pipe, switchMap, tap, throwError } from 'rxjs';
import { Result } from 'neverthrow';

export type AccountLoadingStatus = 'idle' | 'loading' | 'fulfilled' | 'notFound' | 'serverError';

export type AccountCreationStatus = 'idle' | 'creating' | 'fulfilled' | 'conflict' | 'serverError';

export type AccountState = {
  loadingStatus: AccountLoadingStatus;
  creationStatus: AccountCreationStatus;
  account: Account | null;
};

const initialState: AccountState = {
  loadingStatus: 'idle',
  creationStatus: 'idle',
  account: null,
};

export const AccountStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _accountService: inject(AccountService),
  })),

  withProps((store) => ({
    hasAccount: computed(() => store.account() !== null),
  })),

  withMethods((store) => ({
    loadAccount: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loadingStatus: 'loading' })),
        switchMap(() => {
          return store._accountService.getAccount().pipe(
            tap((result: Result<Account, NotFound>) => {
              if (result.isOk()) {
                patchState(store, { account: result.value, loadingStatus: 'fulfilled' });
              } else {
                patchState(store, { account: null, loadingStatus: 'notFound' });
              }
            }),
            catchError((error) => {
              patchState(store, { account: null, loadingStatus: 'serverError' });

              return throwError(() => error);
            }),
          );
        }),
      ),
    ),

    createAccount: rxMethod<string>(
      pipe(
        tap(() => patchState(store, { creationStatus: 'creating' })),
        switchMap((name: string) => {
          return store._accountService.createAccount(name).pipe(
            tap((result: Result<Account, Conflict>) => {
              if (result.isOk()) {
                patchState(store, { account: result.value, creationStatus: 'fulfilled' });
              } else {
                patchState(store, { account: null, creationStatus: 'conflict' });
              }
            }),
            catchError((error) => {
              patchState(store, { account: null, creationStatus: 'serverError' });

              return throwError(() => error);
            }),
          );
        }),
      ),
    ),
  })),
);
