import {
  patchState,
  signalStore,
  withComputed,
  withHooks,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { Game } from '../../models/game';
import { Player } from '../../models/player';
import { Tile } from '../../models/tile';
import { TurnPhase } from '../../models/turn-phase';
import { computed, inject } from '@angular/core';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { EMPTY, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type ActiveGameState = {
  game: Game | null;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  drawnTile: Tile | null;
};

const initialState: ActiveGameState = {
  game: null,
  loadingStatus: 'initial',
  drawnTile: null,
};

export const ActiveGameStore = signalStore(
  withState(initialState),

  withProps(() => ({
    _activeGameService: inject(ActiveGameService),
  })),

  withComputed(({ game, loadingStatus }) => ({
    isLoading: computed(() => loadingStatus() === 'loading'),

    turnPhase: computed<TurnPhase | null>(() => game()?.turnPhase ?? null),

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

    drawTileWhenMyTurn: rxMethod<Player | null>(
      pipe(
        tap(() => patchState(store, { drawnTile: null })),
        switchMap((currentPlayer) => {
          const game = store.game();

          if (!game || !currentPlayer?.isMe || store.turnPhase() !== 'PlaceTile') {
            return EMPTY;
          }

          return store._activeGameService.getNextTile(game.id).pipe(
            tapResponse({
              next: (result) => {
                if (result.isOk()) {
                  patchState(store, { drawnTile: result.value });
                }
              },
              error: () => patchState(store, { drawnTile: null }),
            }),
          );
        }),
      ),
    ),
  })),

  withHooks({
    onInit(store) {
      store.drawTileWhenMyTurn(store.currentPlayer);
    },
  }),
);
