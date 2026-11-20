import { TestBed } from '@angular/core/testing';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { ActiveGameStore } from './active-game-store';
import { of, throwError } from 'rxjs';
import { Game } from '../../models/game';
import { DEFAULT_TEST_GAME, GAME_TESTCASES } from '../../tests/data/game-test-cases';
import { err, ok } from 'neverthrow';

/** Is the authenticated player on turn and has to place a tile? */
function isMyTurnToPlaceTile(game: Game): boolean {
  const currentPlayer = game.players.find(
    (player) => player.turnPosition === game.currentTurnPosition,
  );

  return !!currentPlayer?.isMe && game.turnPhase === 'PlaceTile';
}

describe('ActiveGameStore', () => {
  // dependencies
  const activeGameService = {
    getActiveGame: vi.fn(),
    getNextTile: vi.fn(),
  };

  beforeEach(() => {
    activeGameService.getNextTile.mockReturnValue(of(ok(DEFAULT_TEST_GAME.tiles[0])));

    TestBed.configureTestingModule({
      providers: [ActiveGameStore, { provide: ActiveGameService, useValue: activeGameService }],
    });
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  test('given no active game when loadActiveGame then sets game to null and loadingStatus to loaded', () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    activeGameService.getActiveGame.mockReturnValueOnce(of(null));

    // when
    TestBed.runInInjectionContext(() => {
      store.loadActiveGame();
    });

    // then
    expect(store.loadingStatus()).toEqual('loaded');
    expect(store.game()).toBeNull();
  });

  test('given error when loadActiveGame then sets game to null and loadingStatus to error', () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    activeGameService.getActiveGame.mockReturnValueOnce(throwError(() => new Error('test error')));

    // when
    TestBed.runInInjectionContext(() => {
      store.loadActiveGame();
    });

    // then
    expect(store.loadingStatus()).toEqual('error');
    expect(store.game()).toBeNull();
  });

  test.for<Game>(GAME_TESTCASES)(
    'given active game when loadActiveGame then sets game to value and loadingStatus to loaded',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });

      // then
      expect(store.loadingStatus()).toEqual('loaded');
      expect(store.game()).toEqual(game);
    },
  );

  test('given no active game when currentPlayer then returns null', () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    const currentPlayer = store.currentPlayer();

    // then
    expect(currentPlayer).toBeNull();
  });

  test.for<Game>(GAME_TESTCASES)(
    'given active game when loadActiveGame then currentPlayer is the player at currentTurnPosition',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });

      // then
      const expectedPlayer = game.players.find(
        (player) => player.turnPosition === game.currentTurnPosition,
      );

      expect(expectedPlayer).toBeDefined();
      expect(store.currentPlayer()).toEqual(expectedPlayer);
    },
  );

  // ------------------------------------------------------------------------
  // drawing the next tile
  // ------------------------------------------------------------------------

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnToPlaceTile))(
    'given my turn in place tile phase of game $id when loadActiveGame then draws the next tile and stores it',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);
      const tile = DEFAULT_TEST_GAME.tiles[0];

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      activeGameService.getNextTile.mockReturnValueOnce(of(ok(tile)));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // then
      expect(activeGameService.getNextTile).toHaveBeenCalledExactlyOnceWith(game.id);
      expect(store.drawnTile()).toEqual(tile);
    },
  );

  test.for<Game>(GAME_TESTCASES.filter((game) => !isMyTurnToPlaceTile(game)))(
    'given not my turn or not place tile phase in game $id when loadActiveGame then does not draw a tile',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // then
      expect(activeGameService.getNextTile).not.toHaveBeenCalled();
      expect(store.drawnTile()).toBeNull();
    },
  );
});
