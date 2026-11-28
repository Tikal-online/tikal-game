import { Component, input } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

export type TurnPhase = 'PlaceTile' | 'Actions';

@Component({
  selector: 'tikal-turn-status',
  imports: [TranslocoDirective],
  templateUrl: './turn-status.html',
  styleUrl: './turn-status.scss',
})
export class TurnStatusComponent {
  /** In which phase is the current turn? */
  readonly turnPhase = input.required<TurnPhase>();

  /** @ignore */
  readonly phases: readonly { phase: TurnPhase; key: string }[] = [
    { phase: 'PlaceTile', key: 'place-tile' },
    { phase: 'Actions', key: 'actions' },
  ];
}
