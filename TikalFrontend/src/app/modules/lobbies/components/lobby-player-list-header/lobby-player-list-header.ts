import { Component, input } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideUsers } from '@lucide/angular';

@Component({
  selector: 'app-lobby-player-list-header',
  imports: [TranslocoDirective, LucideUsers],
  templateUrl: './lobby-player-list-header.html',
  styleUrl: './lobby-player-list-header.scss',
})
export class LobbyPlayerListHeader {
  readonly maxPlayers = input.required<number>();
}
