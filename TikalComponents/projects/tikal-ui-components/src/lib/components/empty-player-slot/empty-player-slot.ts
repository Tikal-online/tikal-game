import { Component, input } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { SkeletonComponent } from '../skeleton/skeleton';

@Component({
  selector: 'tikal-empty-player-slot',
  imports: [TranslocoDirective, SkeletonComponent],
  templateUrl: './empty-player-slot.html',
  styleUrl: './empty-player-slot.scss',
})
export class EmptyPlayerSlotComponent {
  /** Is the player currently loading? */
  readonly isLoading = input<boolean>(false);
}
