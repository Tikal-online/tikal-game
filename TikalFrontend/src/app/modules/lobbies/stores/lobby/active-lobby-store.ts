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
import { Router } from '@angular/router';
import { ActiveLobbyService } from '../../services/active-lobby/active-lobby-service';
import { ConnectionStatus } from '../../../../core/enums/connection-status';

type ActiveLobbyState = {
  lobby: Lobby | null;
  status: 'initial' | 'loading' | 'loaded' | 'error';
  connectionStatus: ConnectionStatus;
  leavingStatus: 'initial' | 'leaving' | 'error';
};

const initialState: ActiveLobbyState = {
  lobby: null,
  status: 'initial',
  connectionStatus: 'Disconnected',
  leavingStatus: 'initial',
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
    _router: inject(Router),
  })),

  withComputed(({ status }) => ({
    isLoading: computed(() => status() === 'loading'),
  })),

  withMethods((store) => ({
    connect(): Promise<void> {
      return store._activeLobbyService.connect();
    },

    disconnect(): Promise<void> {
      return store._activeLobbyService.disconnect();
    },

    watchJoinedPlayers: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyService.joinedPlayer$),
        tap((player) =>
          patchState(store, (state) => ({
            lobby: state.lobby
              ? { ...state.lobby, players: [...state.lobby.players, player] }
              : null,
          })),
        ),
      ),
    ),

    watchConnectionStatus: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyService.connectionStatus$),
        tap((status) => patchState(store, { connectionStatus: status })),
      ),
    ),

    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { status: 'loading', leavingStatus: 'initial' })),
        switchMap(() => {
          return store._activeLobbyService.getActiveLobby().pipe(
            tapResponse({
              next: (result) => patchState(store, { lobby: result, status: 'loaded' }),
              error: () => patchState(store, { status: 'error' }),
            }),
          );
        }),
      ),
    ),

    leaveLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { leavingStatus: 'leaving' })),
        switchMap(() => {
          return store._activeLobbyService.leaveLobby().pipe(
            tapResponse({
              next: () => store._router.navigate(['/lobbies']),
              error: () => patchState(store, { leavingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),

  withHooks({
    onInit(store) {
      store.watchJoinedPlayers();
      store.watchConnectionStatus();
    },
  }),
);
