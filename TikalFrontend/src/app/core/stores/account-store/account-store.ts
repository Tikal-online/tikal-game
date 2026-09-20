import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import {
  Account,
  AccountExists,
  AccountService,
} from '../../services/account-service/account-service';
import { computed, inject } from '@angular/core';
import { catchError, firstValueFrom, Observable, tap, throwError } from 'rxjs';
import { Result } from 'neverthrow';

type AccountState = {
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
    loadAccount(): Observable<Account | null> {
      return store._accountService.getAccount().pipe(
        tap((account: Account | null) => {
          patchState(store, { account });
        }),
        catchError((error) => {
          patchState(store, { initializationFailed: true });

          return throwError(() => error);
        }),
      );
    },

    createAccount(name: string): Promise<Result<Account, AccountExists>> {
      const request = store._accountService.createAccount(name).pipe(
        tap((result: Result<Account, AccountExists>) => {
          if (result.isOk()) {
            patchState(store, { account: result.value });
          }
        }),
      );

      return firstValueFrom(request);
    },
  })),
);
