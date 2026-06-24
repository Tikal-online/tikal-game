import { Component, output } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideX } from '@lucide/angular';

@Component({
  selector: 'app-global-chat-header',
  imports: [TranslocoDirective, LucideX],
  templateUrl: './global-chat-header.html',
  styleUrl: './global-chat-header.scss',
})
export class GlobalChatHeader {
  readonly closed = output();
}
