import { Component, input } from '@angular/core';

@Component({
  selector: 'app-global-chat-message',
  imports: [],
  templateUrl: './global-chat-message.html',
  styleUrl: './global-chat-message.scss',
})
export class GlobalChatMessage {
  readonly username = input<string>();
}
