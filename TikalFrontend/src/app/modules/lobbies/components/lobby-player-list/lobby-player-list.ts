import { Component, input } from '@angular/core';
import { Player } from '../../models/player';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideCrown } from '@lucide/angular';

@Component({
  selector: 'app-lobby-player-list',
  imports: [TranslocoDirective, LucideCrown],
  templateUrl: './lobby-player-list.html',
  styleUrl: './lobby-player-list.scss',
})
export class LobbyPlayerList {
  readonly maxPlayers = input.required<number>();

  readonly players = input.required<Player[]>();

  readonly isLoading = input<boolean>(false);
}
