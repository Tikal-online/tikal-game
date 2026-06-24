import { Component, input } from '@angular/core';

@Component({
  selector: 'app-enemy-chat-message',
  imports: [],
  templateUrl: './enemy-chat-message.html',
  styleUrl: './enemy-chat-message.scss',
})
export class EnemyChatMessage {
  readonly username = input<string>();
}
