import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { Account, AccountService, NotFound } from '../../services/account-service/account-service';
import { computed, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Result } from 'neverthrow';

type AccountState = {
  account: Account | null;
};

const initalState: AccountState = {
  account: null,
};

export const AccountStore = signalStore(
  { providedIn: 'root' },
  withState(initalState),
  withComputed(({ account }) => ({
    hasAccount: computed(() => account() !== null),
  })),
  withMethods((store, accountService = inject(AccountService)) => ({
    loadAccount(): Observable<Result<Account, NotFound>> {
      return accountService.getAccount().pipe(
        tap((result) => {
          patchState(store, { account: result.isOk() ? result.value : null });
        }),
      );
    },
  })),
);
