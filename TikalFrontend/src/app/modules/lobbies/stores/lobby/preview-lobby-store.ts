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

type PreviewLobbyState = {
  lobby: Lobby | null;
  status: 'initial' | 'loading' | 'loaded' | 'error';
  joiningStatus:
    'initial' | 'joining' | 'joined' | 'notFound' | 'full' | 'alreadyInLobby' | 'error';
};

const initialStatus: PreviewLobbyState = {
  lobby: null,
  status: 'initial',
  joiningStatus: 'initial',
};

export const PreviewLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialStatus),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
  })),

  withComputed(({ status, joiningStatus }) => ({
    isLoading: computed(() => status() === 'loading'),

    isJoining: computed(() => joiningStatus() === 'joining'),
  })),

  withMethods((store) => ({
    loadLobby: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { status: 'loading', joiningStatus: 'initial' })),
        switchMap((id) => {
          return store._lobbyService.getLobby(id).pipe(
            tapResponse({
              next: (result) => patchState(store, { lobby: result, status: 'loaded' }),
              error: () => patchState(store, { lobby: null, status: 'error' }),
            }),
          );
        }),
      ),
    ),

    joinLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { joiningStatus: 'joining' })),
        switchMap(() => {
          return store._lobbyService.joinLobby(store.lobby()!.id).pipe(
            tapResponse({
              next: (result) => {
                if (result.isErr()) {
                  switch (result.error.type) {
                    case 'PlayerAlreadyInLobby':
                      patchState(store, { joiningStatus: 'alreadyInLobby' });
                      break;
                    case 'LobbyNotFound':
                      patchState(store, { joiningStatus: 'notFound' });
                      break;
                    case 'LobbyFull':
                      patchState(store, { joiningStatus: 'full' });
                      break;
                  }
                } else {
                  patchState(store, { joiningStatus: 'joined' });
                }
              },
              error: () => patchState(store, { joiningStatus: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
