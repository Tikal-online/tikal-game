import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideWifiOff } from '@lucide/angular';

@Component({
  selector: 'app-lobbies-list-error',
  imports: [LucideWifiOff, TranslocoDirective],
  templateUrl: './lobbies-list-error.html',
  styleUrl: './lobbies-list-error.scss',
})
export class LobbiesListError {}
