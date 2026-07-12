import { Component, DestroyRef, inject } from '@angular/core';
import { LucideMessageSquare } from '@lucide/angular';
import { GlobalChatStore } from '../../stores/global-chat/global-chat-store';
import { GlobalChatForm } from './global-chat-form/global-chat-form';
import { GlobalChatDisconnected } from './global-chat-disconnected/global-chat-disconnected';
import { GlobalChatConnecting } from './global-chat-connecting/global-chat-connecting';
import { TranslocoDirective } from '@jsverse/transloco';
import { ChatHeader } from '../chat-header/chat-header';
import { ChatMessages } from '../chat-messages/chat-messages';

@Component({
  selector: 'app-global-chat',
  imports: [
    TranslocoDirective,
    LucideMessageSquare,
    GlobalChatForm,
    GlobalChatDisconnected,
    GlobalChatConnecting,
    ChatHeader,
    ChatMessages,
  ],
  templateUrl: './global-chat.html',
  styleUrl: './global-chat.scss',
})
export class GlobalChat {
  readonly globalChatStore = inject(GlobalChatStore);

  constructor() {
    this.globalChatStore.connect();
    inject(DestroyRef).onDestroy(() => this.globalChatStore.disconnect());
  }
}
