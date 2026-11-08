import { Component, input } from '@angular/core';
import { EnumMapPipe } from '../../../pipes/enum-map';
import { Crown } from '@primeicons/angular/crown';
import { Check } from '@primeicons/angular/check';
import { TranslocoDirective } from '@jsverse/transloco';

export type PlayerColour = 'Red' | 'Black' | 'Green' | 'Blue' | 'Yellow';

@Component({
  selector: 'tikal-player-slot',
  imports: [EnumMapPipe, Crown, Check, TranslocoDirective],
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

  /** Is the player the currently authenticated user? */
  readonly isMe = input.required<boolean>();

  /** @ignore */
  readonly colourMap = {
    Red: 'var(--p-red-500)',
    Black: 'var(--p-surface-900)',
    Green: 'var(--p-green-500)',
    Blue: 'var(--p-blue-500)',
    Yellow: 'var(--p-yellow-500)',
  };
}
