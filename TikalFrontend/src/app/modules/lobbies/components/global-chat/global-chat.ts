import { Component, DestroyRef, inject } from '@angular/core';
import { LucideMessageSquare } from '@lucide/angular';
import { GlobalChatStore } from '../../stores/global-chat/global-chat-store';
import { GlobalChatHeader } from './global-chat-header/global-chat-header';
import { GlobalChatMessages } from './global-chat-messages/global-chat-messages';
import { GlobalChatForm } from './global-chat-form/global-chat-form';

@Component({
  selector: 'app-global-chat',
  imports: [LucideMessageSquare, GlobalChatHeader, GlobalChatMessages, GlobalChatForm],
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
