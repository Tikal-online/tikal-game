import { Component, input } from '@angular/core';
import { EnumMapPipe } from '../../../pipes/enum-map';

export type PlayerColour = 'red' | 'black' | 'green' | 'blue' | 'yellow';

@Component({
  selector: 'tikal-player-slot',
  imports: [EnumMapPipe],
  templateUrl: './player-slot.html',
  styleUrl: './player-slot.scss',
})
export class PlayerSlotComponent {
  /** What is the name of the player? */
  readonly name = input.required<string>();

  /** What is the colour of the player? */
  readonly colour = input.required<PlayerColour>();

  /** @ignore */
  readonly colourMap = {
    red: 'var(--p-red-500)',
    green: 'var(--p-green-500)',
    blue: 'var(--p-blue-500)',
    yellow: 'var(--p-yellow-500)',
  };
}
