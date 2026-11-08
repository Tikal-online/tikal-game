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
import { LoadingStatus } from '../../../../core/types/loading-status';
import { err, Result } from 'neverthrow';
import { LobbyFull, LobbyNotFound, PlayerAlreadyInLobby } from '../../errors/lobby-errors';

type PreviewLobbyState = {
  lobby: Lobby | null;
  status: LoadingStatus;
};

const initialStatus: PreviewLobbyState = {
  lobby: null,
  status: 'initial',
};

export const PreviewLobbyStore = signalStore(
  withState(initialStatus),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
  })),

  withComputed(({ status }) => ({
    isLoading: computed(() => status() === 'loading'),
  })),

  withMethods((store) => ({
    loadLobby: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { status: 'loading' })),
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

    joinLobby(): Promise<Result<void, PlayerAlreadyInLobby | LobbyNotFound | LobbyFull>> {
      const lobby = store.lobby();

      if (!lobby) {
        return Promise.resolve(err({ type: 'LobbyNotFound' } as const));
      }

      return store._lobbyService.joinLobby(lobby.id);
    },
  })),
);
