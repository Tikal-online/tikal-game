import { Component, computed, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { PlayerCardComponent } from 'tikal-ui-components';
import { ActiveGameStore } from '../../stores/active-game/active-game-store';
import { GameBoardComponent } from '../game-board/game-board';

@Component({
  selector: 'tikal-active-game',
  imports: [TranslocoDirective, PlayerCardComponent, GameBoardComponent],
  templateUrl: './active-game.html',
  styleUrl: './active-game.scss',
})
export class ActiveGameComponent {
  private readonly router = inject(Router);

  readonly activeGameStore = inject(ActiveGameStore);

  readonly boardTiles = computed(() => this.activeGameStore.game()?.tiles ?? []);

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
