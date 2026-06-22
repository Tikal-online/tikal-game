import { Component, signal } from '@angular/core';
import { LucideMessageSquare, LucideX } from '@lucide/angular';

@Component({
  selector: 'app-global-chat',
  imports: [LucideX, LucideMessageSquare],
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
