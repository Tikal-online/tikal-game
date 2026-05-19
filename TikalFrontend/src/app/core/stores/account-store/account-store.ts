import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import {
  Account,
  AccountService,
  Conflict,
  NotFound,
} from '../../services/account-service/account-service';
import { computed, inject } from '@angular/core';
import { catchError, firstValueFrom, Observable, tap, throwError } from 'rxjs';
import { Result } from 'neverthrow';

export type AccountState = {
  initializationFailed: boolean;
  account: Account | null;
};

const initialState: AccountState = {
  initializationFailed: false,
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
    // this method returns an observable because it needs to run during app initialization
    loadAccount(): Observable<Result<Account, NotFound>> {
      return store._accountService.getAccount().pipe(
        tap((result: Result<Account, NotFound>) => {
          if (result.isOk()) {
            patchState(store, { account: result.value });
          }
        }),
        catchError((error) => {
          patchState(store, { initializationFailed: true });

          return throwError(() => error);
        }),
      );
    },

    createAccount(name: string): Promise<Result<Account, Conflict>> {
      const request = store._accountService.createAccount(name).pipe(
        tap((result: Result<Account, Conflict>) => {
          if (result.isOk()) {
            patchState(store, { account: result.value });
          }
        }),
      );

      return firstValueFrom(request);
    },
  })),
);
