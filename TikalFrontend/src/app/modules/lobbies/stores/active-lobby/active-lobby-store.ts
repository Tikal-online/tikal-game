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
import { filter, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import {
  ActiveLobbyService,
  CreateLobbyData,
} from '../../services/active-lobby/active-lobby-service';
import { ActiveLobbyConnection } from '../../services/active-lobby/active-lobby-connection';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { ActivePlayerService } from '../../services/active-lobby/active-player-service';
import { Result } from 'neverthrow';
import { PlayerAlreadyInLobby } from '../../errors/lobby-errors';

type ActiveLobbyState = {
  lobby: Lobby | null;
  connectionStatus: ConnectionStatus;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  leavingStatus: 'initial' | 'leaving' | 'left' | 'error';
  isCreating: boolean;
  startingStatus: 'initial' | 'starting' | 'started' | 'error';
  showCreateLobbyDialog: boolean;
};

const initialStatus: ActiveLobbyState = {
  lobby: null,
  connectionStatus: 'Disconnected',
  loadingStatus: 'initial',
  leavingStatus: 'initial',
  isCreating: false,
  startingStatus: 'initial',
  showCreateLobbyDialog: false,
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialStatus),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
    _activePlayerService: inject(ActivePlayerService),
    _activeLobbyConnection: inject(ActiveLobbyConnection),
  })),

  withComputed(({ loadingStatus, leavingStatus, lobby }) => ({
    isLoading: computed(() => loadingStatus() === 'loading'),

    isLeaving: computed(() => leavingStatus() === 'leaving'),

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

    toggleCreateLobbyDialog(): void {
      patchState(store, (state) => ({ showCreateLobbyDialog: !state.showCreateLobbyDialog }));
    },

    readyUp: rxMethod<void>(
      pipe(
        switchMap(() => {
          return store._activePlayerService.readyUp();
        }),
      ),
    ),

    readyDown: rxMethod<void>(
      pipe(
        switchMap(() => {
          return store._activePlayerService.readyDown();
        }),
      ),
    ),

    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() =>
          patchState(store, {
            loadingStatus: 'loading',
            leavingStatus: 'initial',
            startingStatus: 'initial',
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

    leaveActiveLobby: rxMethod<void>(
      pipe(
        filter(() => !!store.lobby()),
        tap(() => patchState(store, { leavingStatus: 'leaving' })),
        switchMap(() => {
          return store._activeLobbyService.leaveLobby(store.lobby()!.id).pipe(
            tapResponse({
              next: () =>
                patchState(store, {
                  lobby: null,
                  leavingStatus: 'left',
                  loadingStatus: 'initial',
                  startingStatus: 'initial',
                }),
              error: () => patchState(store, { leavingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

    startActiveLobby: rxMethod<void>(
      pipe(
        filter(() => !!store.lobby()),
        tap(() => patchState(store, { startingStatus: 'starting' })),
        switchMap(() => {
          return store._activeLobbyService.startLobby(store.lobby()!.id).pipe(
            tapResponse({
              next: () => patchState(store, { startingStatus: 'started' }),
              error: () => patchState(store, { startingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

    createLobby(data: CreateLobbyData): Promise<Result<void, PlayerAlreadyInLobby>> {
      patchState(store, { isCreating: true, showCreateLobbyDialog: false });

      try {
        return store._activeLobbyService.createLobby(data);
      } finally {
        patchState(store, { isCreating: false });
      }
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
              leavingStatus: 'left',
              loadingStatus: 'initial',
              startingStatus: 'initial',
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
