import { Component, input } from '@angular/core';

@Component({
  selector: 'tikal-player-count',
  imports: [],
  templateUrl: './player-count.html',
  styleUrl: './player-count.scss',
})
export class PlayerCountComponent {
  /** How many players are currently in the lobby? */
  readonly currentPlayers = input.required<number>();

  /** How many players fit in the lobby? */
  readonly maxPlayers = input.required<number>();
}
