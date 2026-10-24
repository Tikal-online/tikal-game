import { Component, input } from '@angular/core';
import { ChipModule } from '@openng/optimus-ui/chip';

@Component({
  selector: 'tikal-chip',
  imports: [ChipModule],
  templateUrl: './chip.html',
  styleUrl: './chip.scss',
})
export class ChipComponent {
  /** What icon should the chip display? */
  readonly icon = input<string>('');

  /** What text should the chip display? */
  readonly label = input.required<string>();
}
