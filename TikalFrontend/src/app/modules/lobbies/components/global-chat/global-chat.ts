import { Component, signal } from '@angular/core';
import { LucideMessageSquare, LucideSendHorizontal, LucideX } from '@lucide/angular';

@Component({
  selector: 'app-global-chat',
  imports: [LucideX, LucideMessageSquare, LucideSendHorizontal],
  templateUrl: './global-chat.html',
  styleUrl: './global-chat.scss',
})
export class GlobalChat {
  readonly showChat = signal(true);

  hide(): void {
    this.showChat.set(false);
  }

  show(): void {
    this.showChat.set(true);
  }
}
