import { Component, input, output } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-max-players-selection',
  imports: [TranslocoDirective],
  templateUrl: './max-players-selection.html',
  styleUrl: './max-players-selection.scss',
})
export class MaxPlayersSelection {
  readonly maxPlayers = input.required<number>();

  readonly disabled = input.required<boolean>();

  readonly maxPlayersSet = output<number>();

  setMaxPlayers(maxPlayers: number): void {
    this.maxPlayersSet.emit(maxPlayers);
  }
}
