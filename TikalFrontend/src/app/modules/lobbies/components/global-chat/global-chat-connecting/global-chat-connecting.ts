import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideCircle, LucideLoaderCircle, LucideWifiOff } from '@lucide/angular';

@Component({
  selector: 'app-global-chat-connecting',
  imports: [LucideWifiOff, LucideCircle, LucideLoaderCircle, TranslocoDirective],
  templateUrl: './global-chat-connecting.html',
  styleUrl: './global-chat-connecting.scss',
})
export class GlobalChatConnecting {}
