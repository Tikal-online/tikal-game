import { Component, input } from '@angular/core';
import { EnumMapPipe } from '../../../pipes/enum-map';
import { Crown } from '@primeicons/angular/crown';

export type PlayerColour = 'red' | 'black' | 'green' | 'blue' | 'yellow';

@Component({
  selector: 'tikal-player-slot',
  imports: [EnumMapPipe, Crown],
  templateUrl: './player-slot.html',
  styleUrl: './player-slot.scss',
})
export class PlayerSlotComponent {
  /** What is the name of the player? */
  readonly name = input.required<string>();

  /** What is the colour of the player? */
  readonly colour = input.required<PlayerColour>();

  /** Is the player ready? */
  readonly isReady = input.required<boolean>();

  /** Is the player an owner of the lobby? */
  readonly isOwner = input.required<boolean>();

  /** @ignore */
  readonly colourMap = {
    red: 'var(--p-red-500)',
    green: 'var(--p-green-500)',
    blue: 'var(--p-blue-500)',
    yellow: 'var(--p-yellow-500)',
  };
}
