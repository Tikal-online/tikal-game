import { Component, inject } from '@angular/core';
import { GlobalChatStore } from '../../../stores/global-chat/global-chat-store';
import { LucideRotateCcw, LucideWifiOff } from '@lucide/angular';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-global-chat-disconnected',
  imports: [LucideWifiOff, LucideRotateCcw, TranslocoDirective],
  templateUrl: './global-chat-disconnected.html',
  styleUrl: './global-chat-disconnected.scss',
})
export class GlobalChatDisconnected {
  readonly globalChatStore = inject(GlobalChatStore);
}
