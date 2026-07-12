import { Component, input } from '@angular/core';

@Component({
  selector: 'app-my-chat-message',
  imports: [],
  templateUrl: './my-chat-message.html',
  styleUrl: './my-chat-message.scss',
})
export class MyChatMessage {
  readonly time = input.required<Date>();
}
