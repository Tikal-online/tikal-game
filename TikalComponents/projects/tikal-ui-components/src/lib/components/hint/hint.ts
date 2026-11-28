import { Component, input } from '@angular/core';

@Component({
  selector: 'tikal-hint',
  templateUrl: './hint.html',
  styleUrl: './hint.scss',
})
export class HintComponent {
  /** What text should the hint display? */
  readonly label = input.required<string>();
}
