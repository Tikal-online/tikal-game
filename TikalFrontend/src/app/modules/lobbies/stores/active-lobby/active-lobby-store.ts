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

type ActiveLobbyState = {
  lobby: Lobby | null;
  connectionStatus: ConnectionStatus;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  leavingStatus: 'initial' | 'leaving' | 'left' | 'error';
  creationStatus: 'initial' | 'creating' | 'created' | 'error';
  showCreateLobbyDialog: boolean;
};

const initialStatus: ActiveLobbyState = {
  lobby: null,
  connectionStatus: 'Disconnected',
  loadingStatus: 'initial',
  leavingStatus: 'initial',
  creationStatus: 'initial',
  showCreateLobbyDialog: false,
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialStatus),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
    _activeLobbyConnection: inject(ActiveLobbyConnection),
  })),

  withComputed(({ loadingStatus, leavingStatus, creationStatus, lobby }) => ({
    isLoading: computed(() => loadingStatus() === 'loading'),

    isLeaving: computed(() => leavingStatus() === 'leaving'),

    isCreating: computed(() => creationStatus() === 'creating'),

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

    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() =>
          patchState(store, {
            loadingStatus: 'loading',
            leavingStatus: 'initial',
            creationStatus: 'initial',
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
                  creationStatus: 'initial',
                }),
              error: () => patchState(store, { leavingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

    createLobby: rxMethod<CreateLobbyData>(
      pipe(
        tap(() => patchState(store, { creationStatus: 'creating', showCreateLobbyDialog: false })),
        switchMap((data) => {
          return store._activeLobbyService.createLobby(data).pipe(
            tapResponse({
              next: () => patchState(store, { creationStatus: 'created' }),
              error: () => patchState(store, { creationStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

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
              creationStatus: 'initial',
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
