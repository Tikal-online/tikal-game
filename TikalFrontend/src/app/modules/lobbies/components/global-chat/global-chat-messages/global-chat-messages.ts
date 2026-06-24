import { Component, inject } from '@angular/core';
import { GlobalChatStore } from '../../../stores/global-chat/global-chat-store';
import { MyChatMessage } from './my-chat-message/my-chat-message';
import { EnemyChatMessage } from './enemy-chat-message/enemy-chat-message';
import { ChatMessage } from '../../../services/global-chat/global-chat-service';
import { AuthStore } from '../../../../../core/stores/auth-store/auth-store';

@Component({
  selector: 'app-global-chat-messages',
  imports: [MyChatMessage, EnemyChatMessage],
  templateUrl: './global-chat-messages.html',
  styleUrl: './global-chat-messages.scss',
})
export class GlobalChatMessages {
  private readonly authStore = inject(AuthStore);

  readonly globalChatStore = inject(GlobalChatStore);

  isMyMessage(message: ChatMessage): boolean {
    return message.userId === this.authStore.userId();
  }
}
