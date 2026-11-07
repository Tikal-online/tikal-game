import {
  patchState,
  signalStore,
  withComputed,
  withHooks,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { Lobby } from '../../models/lobby';
import { computed, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import {
  ActiveLobbyService,
  CreateLobbyData,
} from '../../services/active-lobby/active-lobby-service';
import { ActiveLobbyConnection } from '../../services/active-lobby/active-lobby-connection';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { ActivePlayerService } from '../../services/active-lobby/active-player-service';
import { err, Result } from 'neverthrow';
import {
  LobbyInGame,
  LobbyNotFound,
  PlayerAlreadyInLobby,
  PlayerNotInLobby,
} from '../../errors/lobby-errors';

type ActiveLobbyState = {
  lobby: Lobby | null;
  connectionStatus: ConnectionStatus;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
};

const initialStatus: ActiveLobbyState = {
  lobby: null,
  connectionStatus: 'Disconnected',
  loadingStatus: 'initial',
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialStatus),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
    _activePlayerService: inject(ActivePlayerService),
    _activeLobbyConnection: inject(ActiveLobbyConnection),
  })),

  withComputed(({ loadingStatus, lobby }) => ({
    isLoading: computed(() => loadingStatus() === 'loading'),

    activePlayer: computed(() => lobby()?.players.find((p) => p.isMe)),

    isInLobby: computed(() => lobby() !== null),
  })),

  withMethods((store) => ({
    connect(): Promise<void> {
      return store._activeLobbyConnection.connect();
    },

    disconnect(): Promise<void> {
      return store._activeLobbyConnection.disconnect();
    },

    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() =>
          patchState(store, {
            lobby: null,
            loadingStatus: 'loading',
          }),
        ),
        switchMap(() => {
          return store._activeLobbyService.getActiveLobby().pipe(
            tapResponse({
              next: (result) => patchState(store, { lobby: result, loadingStatus: 'loaded' }),
              error: () => patchState(store, { lobby: null, loadingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

    readyUp(): Promise<Result<void, PlayerNotInLobby>> {
      return store._activePlayerService.readyUp();
    },

    readyDown(): Promise<Result<void, PlayerNotInLobby>> {
      return store._activePlayerService.readyDown();
    },

    leaveActiveLobby(): Promise<Result<void, LobbyNotFound | PlayerNotInLobby | LobbyInGame>> {
      const lobby = store.lobby();

      if (!lobby) {
        return Promise.resolve(err({ type: 'PlayerNotInLobby' } as const));
      }

      return store._activeLobbyService.leaveLobby(lobby.id);
    },

    startActiveLobby(): Promise<Result<void, PlayerNotInLobby | LobbyInGame>> {
      const lobby = store.lobby();

      if (!lobby) {
        return Promise.resolve(err({ type: 'PlayerNotInLobby' } as const));
      }

      return store._activeLobbyService.startLobby(lobby.id);
    },

    createLobby(data: CreateLobbyData): Promise<Result<void, PlayerAlreadyInLobby>> {
      return store._activeLobbyService.createLobby(data);
    },

    watchJoinedPlayers: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyConnection.joinedPlayers$),
        tap((player) =>
          patchState(store, (state) => ({
            lobby: state.lobby
              ? { ...state.lobby, players: [...state.lobby.players, player] }
              : null,
          })),
        ),
      ),
    ),

    watchLeftPlayers: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyConnection.leftPlayers$),
        tap((player) => {
          if (player.isMe) {
            patchState(store, {
              lobby: null,
              loadingStatus: 'initial',
            });
          } else {
            patchState(store, (state) => ({
              lobby: state.lobby
                ? {
                    ...state.lobby,
                    players: state.lobby.players.filter((p) => p.userId !== player.userId),
                  }
                : null,
            }));
          }
        }),
      ),
    ),

    watchUpdatedPlayers: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyConnection.updatedPlayers$),
        tap((player) => {
          patchState(store, (state) => ({
            lobby: state.lobby
              ? {
                  ...state.lobby,
                  players: state.lobby.players.map((p) =>
                    p.userId === player.userId ? player : p,
                  ),
                }
              : null,
          }));
        }),
      ),
    ),

    watchConnectionStatus: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyConnection.connectionStatus$),
        tap((status) => patchState(store, { connectionStatus: status })),
      ),
    ),
  })),

  withHooks({
    onInit(store) {
      store.watchJoinedPlayers();
      store.watchConnectionStatus();
      store.watchLeftPlayers();
      store.watchUpdatedPlayers();
    },
  }),
);
