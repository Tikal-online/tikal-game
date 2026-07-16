import { Component, DestroyRef, inject } from '@angular/core';
import { LucideMessageSquare } from '@lucide/angular';
import { GlobalChatStore } from '../../stores/global-chat/global-chat-store';
import { TranslocoDirective } from '@jsverse/transloco';
import { ChatHeader } from '../chat-header/chat-header';
import { ChatMessages } from '../chat-messages/chat-messages';
import { ChatForm } from '../chat-form/chat-form';
import { ChatConnecting } from '../chat-connecting/chat-connecting';
import { ChatDisconnected } from '../chat-disconnected/chat-disconnected';

@Component({
  selector: 'app-global-chat',
  imports: [
    TranslocoDirective,
    LucideMessageSquare,
    ChatHeader,
    ChatMessages,
    ChatForm,
    ChatConnecting,
    ChatDisconnected,
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
