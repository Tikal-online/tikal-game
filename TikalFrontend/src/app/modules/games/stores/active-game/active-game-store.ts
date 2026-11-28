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
import { HexCoordinate, rotateCosts, Tile } from '../../models/tile';
import { TurnPhase } from '../../models/turn-phase';
import { computed, inject } from '@angular/core';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { EDGE_OFFSETS } from '../../board/hex-layout';
import { ActiveGameConnection } from '../../services/active-game/active-game-connection';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { EMPTY, filter, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type ActiveGameState = {
  game: Game | null;
  connectionStatus: ConnectionStatus;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  drawnTile: Tile | null;
  /** 60 degree steps the drawn tile is turned clockwise (0 to 5), the tile keeps its original costs. */
  drawnTileRotation: number;
  isCompleted: boolean;
};

const initialState: ActiveGameState = {
  game: null,
  connectionStatus: 'Disconnected',
  loadingStatus: 'initial',
  drawnTile: null,
  drawnTileRotation: 0,
  isCompleted: false,
};

function isNextToPlacedTile(tiles: readonly Tile[], coordinate: HexCoordinate): boolean {
  return Object.values(EDGE_OFFSETS).some((offset) =>
    tiles.some(
      (tile) =>
        tile.coordinate.q === coordinate.q + offset.q &&
        tile.coordinate.r === coordinate.r + offset.r,
    ),
  );
}

export const ActiveGameStore = signalStore(
  withState(initialState),

  withProps(() => ({
    _activeGameService: inject(ActiveGameService),
    _activeGameConnection: inject(ActiveGameConnection),
  })),

  withComputed(({ game, drawnTile, drawnTileRotation, loadingStatus }) => ({
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

    rotatedDrawnTile: computed<Tile | null>(() => {
      const tile = drawnTile();

      return tile ? { ...tile, costs: rotateCosts(tile.costs, drawnTileRotation()) } : null;
    }),

    canPlaceDrawnTile: computed<boolean>(() => {
      const activeGame = game();
      const tile = drawnTile();

      return !!activeGame && !!tile && isNextToPlacedTile(activeGame.tiles, tile.coordinate);
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
          patchState(store, { game: null, loadingStatus: 'loading', isCompleted: false });
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

    moveDrawnTile(coordinate: HexCoordinate): void {
      patchState(store, (state) => ({
        drawnTile: state.drawnTile ? { ...state.drawnTile, coordinate } : null,
      }));
    },

    async placeDrawnTile(coordinate: HexCoordinate): Promise<void> {
      const game = store.game();
      const rotatedTile = store.rotatedDrawnTile();

      if (!game || !rotatedTile || !isNextToPlacedTile(game.tiles, coordinate)) {
        return;
      }

      const result = await store._activeGameService.placeTile(
        game.id,
        coordinate,
        store.drawnTileRotation(),
      );

      if (result.isOk()) {
        const placedTile: Tile = { ...rotatedTile, coordinate };

        patchState(store, (state) => ({
          game: state.game
            ? {
                ...state.game,
                tiles: state.game.tiles.some(
                  (tile) =>
                    tile.coordinate.q === coordinate.q && tile.coordinate.r === coordinate.r,
                )
                  ? state.game.tiles
                  : [...state.game.tiles, placedTile],
                turnPhase: 'Actions' as const,
              }
            : null,
          drawnTile: null,
          drawnTileRotation: 0,
        }));
      }
    },

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

    watchLeftPlayers: rxMethod<void>(
      pipe(
        switchMap(() => store._activeGameConnection.leftPlayers$),
        tap((player) => {
          if (player.isMe) {
            patchState(store, {
              game: null,
              loadingStatus: 'initial',
            });
          } else {
            patchState(store, (state) => ({
              game: state.game
                ? {
                    ...state.game,
                    players: state.game.players.filter((p) => p.userId !== player.userId),
                  }
                : null,
            }));
          }
        }),
      ),
    ),

    watchGameCompleted: rxMethod<void>(
      pipe(
        switchMap(() => store._activeGameConnection.gameCompleted$),
        tap(() =>
          patchState(store, (state) => ({
            ...initialState,
            connectionStatus: state.connectionStatus,
            isCompleted: true,
          })),
        ),
      ),
    ),

    watchTurnCompleted: rxMethod<void>(
      pipe(
        switchMap(() => store._activeGameConnection.turnCompleted$),
        tap((nextPlayer) =>
          patchState(store, (state) => ({
            game: state.game
              ? {
                  ...state.game,
                  currentTurnPosition: nextPlayer.turnPosition,
                  turnPhase: 'PlaceTile' as const,
                }
              : null,
          })),
        ),
      ),
    ),

    watchTilePlaced: rxMethod<void>(
      pipe(
        switchMap(() => store._activeGameConnection.tilePlaced$),
        // a place that already has a tile keeps it, the message is ignored
        filter(
          (tile) =>
            !store
              .game()
              ?.tiles.some(
                (placed) =>
                  placed.coordinate.q === tile.coordinate.q &&
                  placed.coordinate.r === tile.coordinate.r,
              ),
        ),
        // placing the tile ends the place tile phase
        tap((tile) =>
          patchState(store, (state) => ({
            game: state.game
              ? {
                  ...state.game,
                  tiles: [...state.game.tiles, tile],
                  turnPhase: 'Actions' as const,
                }
              : null,
            drawnTile: null,
            drawnTileRotation: 0,
          })),
        ),
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
      store.watchLeftPlayers();
      store.watchTurnCompleted();
      store.watchTilePlaced();
      store.watchGameCompleted();
    },
  }),
);
