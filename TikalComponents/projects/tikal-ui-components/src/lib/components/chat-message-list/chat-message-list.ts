import { Component, input } from '@angular/core';
import { ScrollerModule } from 'primeng/scroller';
import { ChatMessageOrigin, ChatMessageComponent } from '../chat-message/chat-message';

export type ChatMessageData = {
  message: string;
  origin: ChatMessageOrigin;
};

@Component({
  selector: 'tikal-chat-message-list',
  imports: [ScrollerModule, ChatMessageComponent],
  templateUrl: './chat-message-list.html',
  styleUrl: './chat-message-list.scss',
})
export class ChatMessageListComponent {
  /** What messages have been sent? */
  readonly messages = input<ChatMessageData[]>([]);
}
