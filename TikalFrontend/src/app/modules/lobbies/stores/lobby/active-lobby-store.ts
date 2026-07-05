import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { Lobby } from '../../models/lobby';
import { computed, inject } from '@angular/core';
import { LobbyService } from '../../services/lobby/lobby-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type ActiveLobbyState = {
  lobby: Lobby | null;
  status: 'initial' | 'loading' | 'loaded' | 'error';
};

const initialState: ActiveLobbyState = {
  lobby: null,
  status: 'initial',
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
  })),

  withComputed(({ status }) => ({
    isLoading: computed(() => status() === 'loading'),
  })),

  withMethods((store) => ({
    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { status: 'loading' })),
        switchMap(() => {
          return store._lobbyService.getActiveLobby().pipe(
            tapResponse({
              next: (result) =>
                patchState(store, { lobby: result.isOk() ? result.value : null, status: 'loaded' }),
              error: () => patchState(store, { status: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
