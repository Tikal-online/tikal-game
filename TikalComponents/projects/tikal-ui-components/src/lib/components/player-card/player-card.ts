import { Component, computed, input } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { PlayerColour } from '../player-slot/player-slot';

@Component({
  selector: 'tikal-player-card',
  imports: [TranslocoDirective],
  templateUrl: './player-card.html',
  styleUrl: './player-card.scss',
  host: {
    '[class.me]': 'isMe()',
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

  /** @ignore */
  readonly colourMap: Record<PlayerColour, string> = {
    red: 'var(--p-red-500)',
    black: 'var(--p-surface-900)',
    green: 'var(--p-green-500)',
    blue: 'var(--p-blue-500)',
    yellow: 'var(--p-yellow-500)',
  };

  /** @ignore */
  readonly colourValue = computed(() => this.colourMap[this.colour()]);
}
