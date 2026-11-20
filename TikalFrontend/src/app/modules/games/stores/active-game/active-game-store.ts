import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { Game } from '../../models/game';
import { Player } from '../../models/player';
import { computed, inject } from '@angular/core';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type ActiveGameState = {
  game: Game | null;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
};

const initialState: ActiveGameState = {
  game: null,
  loadingStatus: 'initial',
};

export const ActiveGameStore = signalStore(
  withState(initialState),

  withProps(() => ({
    _activeGameService: inject(ActiveGameService),
  })),

  withComputed(({ game, loadingStatus }) => ({
    isLoading: computed(() => loadingStatus() === 'loading'),

    currentPlayer: computed<Player | null>(() => {
      const activeGame = game();

      if (!activeGame) {
        return null;
      }

      return activeGame.players.find(
        (player) => player.turnPosition === activeGame.currentTurnPosition,
      )!;
    }),
  })),

  withMethods((store) => ({
    loadActiveGame: rxMethod<void>(
      pipe(
        tap(() => {
          patchState(store, { game: null, loadingStatus: 'loading' });
        }),
        switchMap(() => {
          return store._activeGameService.getActiveGame().pipe(
            tapResponse({
              next: (result) => patchState(store, { game: result, loadingStatus: 'loaded' }),
              error: () => patchState(store, { game: null, loadingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
