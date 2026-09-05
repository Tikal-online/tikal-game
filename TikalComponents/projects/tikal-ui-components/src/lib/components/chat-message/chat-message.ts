import { Component, input } from '@angular/core';
import { MessageModule } from 'primeng/message';
import { EnumMapPipe } from '../../../pipes/enum-map';

export type ChatMessageOrigin = 'Me' | 'Opponent';

@Component({
  selector: 'tikal-chat-message',
  imports: [MessageModule, EnumMapPipe],
  templateUrl: './chat-message.html',
  styleUrl: './chat-message.scss',
})
export class ChatMessageComponent {
  /** What is the message that should be displayed? */
  readonly message = input.required<string>();

  /** Who sent the message? */
  readonly sender = input<ChatMessageOrigin>('Opponent');

  /** @ignore */
  readonly typeMap = {
    Me: 'contrast',
    Opponent: 'secondary',
  };
}
