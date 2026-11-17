import { Component, computed, input } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { Hourglass } from '@primeicons/angular/hourglass';
import { PlayerColour } from '../player-slot/player-slot';

@Component({
  selector: 'tikal-player-card',
  imports: [TranslocoDirective, Hourglass],
  templateUrl: './player-card.html',
  styleUrl: './player-card.scss',
  host: {
    '[class.me]': 'isMe()',
    '[class.current-turn]': 'isCurrentTurn()',
    '[style.--player-colour]': 'colourValue()',
  },
})
export class PlayerCardComponent {
  /** What is the name of the player? */
  readonly name = input.required<string>();

  /** What is the colour of the player? */
  readonly colour = input.required<PlayerColour>();

  /** How many points does the player have? */
  readonly points = input.required<number>();

  /** Is the player the currently authenticated user? */
  readonly isMe = input.required<boolean>();

  /** Is it currently this player's turn? */
  readonly isCurrentTurn = input<boolean>(false);

  /** @ignore */
  readonly colourMap: Record<PlayerColour, string> = {
    Red: 'var(--p-red-500)',
    Black: 'var(--p-surface-900)',
    Green: 'var(--p-green-500)',
    Blue: 'var(--p-blue-500)',
    Yellow: 'var(--p-yellow-500)',
  };

  /** @ignore */
  readonly colourValue = computed(() => this.colourMap[this.colour()]);
}
