import { Component, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { PlayerCardComponent } from 'tikal-ui-components';
import { ActiveGameStore } from '../../stores/active-game/active-game-store';
import { GameBoardComponent } from '../game-board/game-board';
import { placeholderTiles } from '../../board/placeholder-tiles';

@Component({
  selector: 'tikal-active-game',
  imports: [TranslocoDirective, PlayerCardComponent, GameBoardComponent],
  templateUrl: './active-game.html',
  styleUrl: './active-game.scss',
})
export class ActiveGameComponent {
  private readonly router = inject(Router);

  readonly activeGameStore = inject(ActiveGameStore);

  // until the backend sends the whole board, game.tiles only holds a few tiles
  readonly boardTiles = placeholderTiles();

  constructor() {
    this.activeGameStore.loadActiveGame();

    // covers having no active game on page load
    effect(() => {
      if (!this.activeGameStore.game() && !this.activeGameStore.isLoading()) {
        this.router.navigate(['/Lobbies']);
      }
    });
  }
}
