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
import { catchError, firstValueFrom, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { Router } from '@angular/router';
import { ActiveLobbyService } from '../../services/active-lobby/active-lobby-service';
import { ConnectionStatus } from '../../../../core/enums/connection-status';
import { AccountStore } from '../../../../core/stores/account-store/account-store';
import { ChatMessage } from '../../models/chat-message';

type ActiveLobbyState = {
  lobby: Lobby | null;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  connectionStatus: ConnectionStatus;
  leavingStatus: 'initial' | 'leaving' | 'error';
  showLobbyChat: boolean;
  messages: ChatMessage[];
};

const initialState: ActiveLobbyState = {
  lobby: null,
  loadingStatus: 'initial',
  connectionStatus: 'Disconnected',
  leavingStatus: 'initial',
  showLobbyChat: true,
  messages: [],
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
    _accountStore: inject(AccountStore),
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
      return firstValueFrom(store._activeLobbyService.sendMessage(message));
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
          if (store._accountStore.isMe(player.userId)) {
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
              next: (result) => patchState(store, { lobby: result, loadingStatus: 'loaded' }),
              error: () => patchState(store, { loadingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),

    leaveLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { leavingStatus: 'leaving' })),
        switchMap(() => {
          return store._activeLobbyService
            .leaveLobby()
            .pipe(catchError(async () => patchState(store, { leavingStatus: 'error' })));
        }),
      ),
    ),
  })),

  withHooks({
    onInit(store) {
      store.watchJoinedPlayers();
      store.watchConnectionStatus();
      store.watchLeftPlayers();
      store.watchMessages();
    },
  }),
);
