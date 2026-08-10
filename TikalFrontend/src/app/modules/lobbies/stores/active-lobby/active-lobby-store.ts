import {
  patchState,
  signalStore,
  withHooks,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { Lobby } from '../../models/lobby';
import { inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { catchError, filter, firstValueFrom, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { Router } from '@angular/router';
import { ActiveLobbyService } from '../../services/active-lobby/active-lobby-service';
import { ConnectionStatus } from '../../../../core/enums/connection-status';
import { ChatMessage } from '../../models/chat-message';

type ActiveLobbyState = {
  lobby: Lobby | null;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  connectionStatus: ConnectionStatus;
  leavingStatus: 'initial' | 'leaving' | 'error';
  readyingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  showLobbyChat: boolean;
  messages: ChatMessage[];
};

const initialState: ActiveLobbyState = {
  lobby: null,
  loadingStatus: 'initial',
  connectionStatus: 'Disconnected',
  leavingStatus: 'initial',
  readyingStatus: 'initial',
  showLobbyChat: true,
  messages: [],
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
    _router: inject(Router),
  })),

  withMethods((store) => ({
    connect(): Promise<void> {
      return store._activeLobbyService.connect();
    },

    disconnect(): Promise<void> {
      return store._activeLobbyService.disconnect();
    },

    showChat(): void {
      patchState(store, { showLobbyChat: true });
    },

    hideChat(): void {
      patchState(store, { showLobbyChat: false });
    },

    sendMessage(message: string): Promise<void> {
      const id = store.lobby()?.id;

      if (!id) {
        return Promise.resolve();
      }

      return firstValueFrom(store._activeLobbyService.sendMessage(id, message));
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

    watchLeftPlayers: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyService.leftPlayers$),
        tap((player) => {
          if (player.isMe) {
            store._router.navigate(['/lobbies']);
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
        switchMap(() => store._activeLobbyService.updatedPlayers$),
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
        switchMap(() => store._activeLobbyService.connectionStatus$),
        tap((status) => patchState(store, { connectionStatus: status })),
      ),
    ),

    watchMessages: rxMethod<void>(
      pipe(
        switchMap(() => store._activeLobbyService.message$),
        tap((message) =>
          patchState(store, (state) => ({ messages: [message, ...state.messages] })),
        ),
      ),
    ),

    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() =>
          patchState(store, {
            loadingStatus: 'loading',
            leavingStatus: 'initial',
            showLobbyChat: true,
            messages: [],
          }),
        ),
        switchMap(() => {
          return store._activeLobbyService.getActiveLobby().pipe(
            tapResponse({
              next: (result) =>
                patchState(store, {
                  lobby: result,
                  loadingStatus: 'loaded',
                }),
              error: () => patchState(store, { loadingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

    leaveLobby: rxMethod<void>(
      pipe(
        filter(() => !!store.lobby()),
        tap(() => patchState(store, { leavingStatus: 'leaving' })),
        switchMap(() => {
          return store._activeLobbyService
            .leaveLobby(store.lobby()!.id)
            .pipe(catchError(async () => patchState(store, { leavingStatus: 'error' })));
        }),
      ),
    ),

    readyUp: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { readyingStatus: 'loading' })),
        switchMap(() => {
          return store._activeLobbyService.readyUp().pipe(
            tapResponse({
              next: () => patchState(store, { readyingStatus: 'loaded' }),
              error: () => patchState(store, { readyingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

    readyDown: rxMethod<void>(
      pipe(
        filter(() => !!store.lobby()),
        tap(() => patchState(store, { readyingStatus: 'loading' })),
        switchMap(() => {
          return store._activeLobbyService.readyDown().pipe(
            tapResponse({
              next: () => patchState(store, { readyingStatus: 'loaded' }),
              error: () => patchState(store, { readyingStatus: 'error' }),
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
      store.watchLeftPlayers();
      store.watchUpdatedPlayers();
      store.watchMessages();
    },
  }),
);
