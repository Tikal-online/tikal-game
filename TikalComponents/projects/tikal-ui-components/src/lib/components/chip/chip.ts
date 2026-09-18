import { Component, input } from '@angular/core';
import { ChipModule } from 'primeng/chip';
import { PIcon } from '@primeicons/angular';

@Component({
  selector: 'tikal-chip',
  imports: [ChipModule, PIcon],
  templateUrl: './chip.html',
  styleUrl: './chip.scss',
})
export class ChipComponent {
  /** What icon should the chip display? */
  readonly icon = input<string>();

  /** What text should the chip display? */
  readonly label = input.required<string>();
}
