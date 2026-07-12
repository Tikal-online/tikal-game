import { Component, inject, input } from '@angular/core';
import { ChatMessage } from '../../services/global-chat/global-chat-service';
import { AccountStore } from '../../../../core/stores/account-store/account-store';
import { MyChatMessage } from './my-chat-message/my-chat-message';
import { EnemyChatMessage } from './enemy-chat-message/enemy-chat-message';

@Component({
  selector: 'app-chat-messages',
  imports: [MyChatMessage, EnemyChatMessage],
  templateUrl: './chat-messages.html',
  styleUrl: './chat-messages.scss',
})
export class ChatMessages {
  readonly accountStore = inject(AccountStore);

  readonly messages = input.required<ChatMessage[]>();
}
