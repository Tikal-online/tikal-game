import { Component, input } from '@angular/core';
import { PlayerColour, PlayerSlotComponent } from '../player-slot/player-slot';
import { EmptyPlayerSlotComponent } from '../empty-player-slot/empty-player-slot';
import { User } from '@primeicons/angular';
import { TranslocoDirective } from '@jsverse/transloco';

export type PlayerSlotData = {
  name: string;
  colour: PlayerColour;
};

@Component({
  selector: 'tikal-player-slot-list',
  imports: [PlayerSlotComponent, EmptyPlayerSlotComponent, User, TranslocoDirective],
  templateUrl: './player-slot-list.html',
  styleUrl: './player-slot-list.scss',
})
export class PlayerSlotListComponent {
  /** How many players fit in the lobby? */
  readonly maxPlayers = input.required<number>();

  /** What players are currently in the lobby? */
  readonly players = input.required<PlayerSlotData[]>();

  /** Are the players currenlty loading? */
  readonly isLoading = input<boolean>(false);
}
