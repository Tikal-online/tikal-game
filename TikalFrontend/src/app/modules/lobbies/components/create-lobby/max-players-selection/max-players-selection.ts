import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-max-players-selection',
  imports: [],
  templateUrl: './max-players-selection.html',
  styleUrl: './max-players-selection.scss',
})
export class MaxPlayersSelection {
  readonly maxPlayers = input.required<number>();

  readonly maxPlayersSet = output<number>();

  setMaxPlayers(maxPlayers: number): void {
    this.maxPlayersSet.emit(maxPlayers);
  }
}
