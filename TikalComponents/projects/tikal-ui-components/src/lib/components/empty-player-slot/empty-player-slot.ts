import { Component, input } from '@angular/core';

@Component({
  selector: 'tikal-empty-player-slot',
  imports: [],
  templateUrl: './empty-player-slot.html',
  styleUrl: './empty-player-slot.scss',
})
export class EmptyPlayerSlotComponent {
  /** What text should be displayed? */
  readonly text = input.required<string>();
}
