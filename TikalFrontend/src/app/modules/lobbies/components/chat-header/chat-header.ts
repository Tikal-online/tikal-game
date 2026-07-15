import { Component, input, output } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideX, LucideWifi, LucideWifiOff } from '@lucide/angular';
import { ConnectionStatus } from '../../../../core/enums/connection-status';

@Component({
  selector: 'app-chat-header',
  imports: [TranslocoDirective, LucideX, LucideWifi, LucideWifiOff],
  templateUrl: './chat-header.html',
  styleUrl: './chat-header.scss',
})
export class ChatHeader {
  readonly header = input.required<string>();

  readonly status = input.required<ConnectionStatus>();

  readonly closed = output<void>();
}
