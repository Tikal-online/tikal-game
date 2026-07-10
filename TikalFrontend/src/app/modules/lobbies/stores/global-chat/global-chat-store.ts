import { inject } from '@angular/core';
import {
  patchState,
  signalStore,
  withHooks,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { ChatMessage, GlobalChatService } from '../../services/global-chat/global-chat-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { ConnectionStatus } from '../../../../core/enums/connection-status';

export type GlobalChatState = {
  status: ConnectionStatus;
  messages: ChatMessage[];
  isExpanded: boolean;
};

const initialState: GlobalChatState = {
  status: 'Disconnected',
  messages: [],
  isExpanded: true,
};

export const GlobalChatStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _globalChatService: inject(GlobalChatService),
  })),

  withMethods((store) => ({
    expand(): void {
      patchState(store, { isExpanded: true });
    },

    collapse(): void {
      patchState(store, { isExpanded: false });
    },

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
