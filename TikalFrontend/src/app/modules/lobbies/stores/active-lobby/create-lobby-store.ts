import { computed, inject } from '@angular/core';
import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import {
  ActiveLobbyService,
  CreateLobbyData,
} from '../../services/active-lobby/active-lobby-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type CreateLobbyState = {
  showCreateLobbyDialog: boolean;
  status: 'initial' | 'creating' | 'created' | 'error';
};

const initialStatus: CreateLobbyState = {
  showCreateLobbyDialog: false,
  status: 'initial',
};

export const CreateLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialStatus),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
  })),

  withComputed(({ status }) => ({
    isLoading: computed(() => status() === 'creating'),
  })),

  withMethods((store) => ({
    toggleCreateLobbyDialog(): void {
      patchState(store, (state) => ({ showCreateLobbyDialog: !state.showCreateLobbyDialog }));
    },

    createLobby: rxMethod<CreateLobbyData>(
      pipe(
        tap(() => patchState(store, { status: 'creating' })),
        switchMap((data) => {
          return store._activeLobbyService.createLobby(data).pipe(
            tapResponse({
              next: () => patchState(store, { status: 'created' }),
              error: () => patchState(store, { status: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
