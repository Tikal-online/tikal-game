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
import { rotateCosts, Tile } from '../../models/tile';
import { TurnPhase } from '../../models/turn-phase';
import { computed, inject } from '@angular/core';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { ActiveGameConnection } from '../../services/active-game/active-game-connection';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { EMPTY, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type ActiveGameState = {
  game: Game | null;
  connectionStatus: ConnectionStatus;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  drawnTile: Tile | null;
  /** 60 degree steps the drawn tile is turned clockwise (0 to 5), the tile keeps its original costs. */
  drawnTileRotation: number;
};

const initialState: ActiveGameState = {
  game: null,
  connectionStatus: 'Disconnected',
  loadingStatus: 'initial',
  drawnTile: null,
  drawnTileRotation: 0,
};

export const ActiveGameStore = signalStore(
  withState(initialState),

  withProps(() => ({
    _activeGameService: inject(ActiveGameService),
    _activeGameConnection: inject(ActiveGameConnection),
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

  withComputed(({ drawnTile, drawnTileRotation }) => ({
    rotatedDrawnTile: computed<Tile | null>(() => {
      const tile = drawnTile();

      return tile ? { ...tile, costs: rotateCosts(tile.costs, drawnTileRotation()) } : null;
    }),
  })),

  withMethods((store) => ({
    connect(): Promise<void> {
      return store._activeGameConnection.connect();
    },

    disconnect(): Promise<void> {
      return store._activeGameConnection.disconnect();
    },

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

    rotateDrawnTile(steps: number): void {
      if (!store.drawnTile()) {
        return;
      }

      patchState(store, {
        drawnTileRotation: (((store.drawnTileRotation() + steps) % 6) + 6) % 6,
      });
    },

    drawTileWhenMyTurn: rxMethod<Player | null>(
      pipe(
        tap(() => patchState(store, { drawnTile: null, drawnTileRotation: 0 })),
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

    watchConnectionStatus: rxMethod<void>(
      pipe(
        switchMap(() => store._activeGameConnection.connectionStatus$),
        tap((status) => patchState(store, { connectionStatus: status })),
      ),
    ),
  })),

  withHooks({
    onInit(store) {
      store.drawTileWhenMyTurn(store.currentPlayer);
      store.watchConnectionStatus();
    },
  }),
);
