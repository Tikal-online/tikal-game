import { Component, computed, DestroyRef, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { PlayerCardComponent, TurnStatusComponent } from 'tikal-ui-components';
import { ActiveGameStore } from '../../stores/active-game/active-game-store';
import { GameBoardComponent } from '../game-board/game-board';

@Component({
  selector: 'tikal-active-game',
  imports: [TranslocoDirective, PlayerCardComponent, TurnStatusComponent, GameBoardComponent],
  templateUrl: './active-game.html',
  styleUrl: './active-game.scss',
})
export class ActiveGameComponent {
  private readonly router = inject(Router);

  readonly activeGameStore = inject(ActiveGameStore);

  readonly boardTiles = computed(() => this.activeGameStore.game()?.tiles ?? []);

  constructor() {
    this.activeGameStore.loadActiveGame();
    this.activeGameStore.connect();

    inject(DestroyRef).onDestroy(() => this.activeGameStore.disconnect());

    effect(() => {
      // the game is over, the players go back to their lobby
      if (this.activeGameStore.isCompleted()) {
        this.router.navigate(['/Lobbies/me']);
        return;
      }

      // covers having no active game on page load and leaving the game
      if (!this.activeGameStore.game() && !this.activeGameStore.isLoading()) {
        this.router.navigate(['/Lobbies']);
      }
    });
  }
}
