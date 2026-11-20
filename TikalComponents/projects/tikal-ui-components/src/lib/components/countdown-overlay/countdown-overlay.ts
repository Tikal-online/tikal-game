import { Component, input } from '@angular/core';

@Component({
  selector: 'tikal-countdown-overlay',
  templateUrl: './countdown-overlay.html',
  styleUrl: './countdown-overlay.scss',
})
export class CountdownOverlayComponent {
  /** How many seconds are left? 0 shows the final label instead of the number. */
  readonly secondsLeft = input.required<number>();

  /** What text is shown above the number while counting down? */
  readonly label = input.required<string>();

  /** What text is shown once the countdown has reached 0? */
  readonly finalLabel = input.required<string>();
}
