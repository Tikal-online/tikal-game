import { Component, inject } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideWifi, LucideWifiOff, LucideX } from '@lucide/angular';
import { GlobalChatStore } from '../../../stores/global-chat/global-chat-store';

@Component({
  selector: 'app-global-chat-header',
  imports: [TranslocoDirective, LucideX, LucideWifi, LucideWifiOff],
  templateUrl: './global-chat-header.html',
  styleUrl: './global-chat-header.scss',
})
export class GlobalChatHeader {
  readonly globalChatStore = inject(GlobalChatStore);
}
