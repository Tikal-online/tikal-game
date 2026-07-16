import { Component, output } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideRotateCcw, LucideWifiOff } from '@lucide/angular';

@Component({
  selector: 'app-chat-disconnected',
  imports: [LucideWifiOff, LucideRotateCcw, TranslocoDirective],
  templateUrl: './chat-disconnected.html',
  styleUrl: './chat-disconnected.scss',
})
export class ChatDisconnected {
  readonly connect = output<void>();
}
