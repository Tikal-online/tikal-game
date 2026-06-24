import { inject } from '@angular/core';
import {
  patchState,
  signalStore,
  withHooks,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import {
  ChatMessage,
  ConnectionStatus,
  GlobalChatService,
} from '../../services/global-chat/global-chat-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';

export type GlobalChatState = {
  status: ConnectionStatus;
  messages: ChatMessage[];
};

const initialState: GlobalChatState = {
  status: 'Disconnected',
  messages: [],
};

export const GlobalChatStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _globalChatService: inject(GlobalChatService),
  })),

  withMethods((store) => ({
    connect(): Promise<void> {
      return store._globalChatService.connect();
    },

    disconnect(): Promise<void> {
      return store._globalChatService.disconnect();
    },

    sendMessage(message: string): Promise<void> {
      return store._globalChatService.sendMessage(message);
    },

    watchMessages: rxMethod<void>(
      pipe(
        switchMap(() => store._globalChatService.message$),
        tap((message) => patchState(store, { messages: [message, ...store.messages()] })),
      ),
    ),

    watchConnectionStatus: rxMethod<void>(
      pipe(
        switchMap(() => store._globalChatService.connectionStatus$),
        tap((status) => patchState(store, { status: status })),
      ),
    ),
  })),

  withHooks({
    onInit(store) {
      store.watchMessages();
      store.watchConnectionStatus();
    },
  }),
);
