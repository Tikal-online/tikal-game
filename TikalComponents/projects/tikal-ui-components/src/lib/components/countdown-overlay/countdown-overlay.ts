import { Component, input } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'tikal-countdown-overlay',
  imports: [TranslocoDirective],
  templateUrl: './countdown-overlay.html',
  styleUrl: './countdown-overlay.scss',
})
export class CountdownOverlayComponent {
  /** How many seconds are left until the game starts? 0 shows that it is starting now. */
  readonly secondsLeft = input.required<number>();
}
