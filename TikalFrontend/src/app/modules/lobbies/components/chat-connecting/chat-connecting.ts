import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideCircle, LucideLoaderCircle, LucideWifiOff } from '@lucide/angular';

@Component({
  selector: 'app-chat-connecting',
  imports: [LucideWifiOff, LucideCircle, LucideLoaderCircle, TranslocoDirective],
  templateUrl: './chat-connecting.html',
  styleUrl: './chat-connecting.scss',
})
export class ChatConnecting {}
