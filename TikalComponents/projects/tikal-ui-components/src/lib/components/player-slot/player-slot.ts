import { Component, input } from '@angular/core';

@Component({
  selector: 'tikal-player-slot',
  imports: [],
  templateUrl: './player-slot.html',
  styleUrl: './player-slot.scss',
})
export class PlayerSlotComponent {
  /** What is the name of the player? */
  readonly name = input.required<string>();
}
