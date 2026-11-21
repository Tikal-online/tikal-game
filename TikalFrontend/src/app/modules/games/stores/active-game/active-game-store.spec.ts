import { TestBed } from '@angular/core/testing';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { ActiveGameStore } from './active-game-store';
import { ActiveGameConnection } from '../../services/active-game/active-game-connection';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { Player } from '../../models/player';
import { of, Subject, throwError } from 'rxjs';
import { Game } from '../../models/game';
import { DEFAULT_TEST_GAME, GAME_TESTCASES } from '../../tests/data/game-test-cases';
import { ok } from 'neverthrow';
import { PLAYER_TESTCASES } from '../../tests/data/player-test-cases';

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

  const activeGameConnection = {
    connect: vi.fn(),
    disconnect: vi.fn(),
    connectionStatus$: new Subject<ConnectionStatus>(),
    leftPlayers$: new Subject<Player>(),
    turnCompleted$: new Subject<Player>(),
    gameCompleted$: new Subject<void>(),
  };

  beforeEach(() => {
    activeGameService.getNextTile.mockReturnValue(of(ok(DEFAULT_TEST_GAME.tiles[0])));

    TestBed.configureTestingModule({
      providers: [
        ActiveGameStore,
        { provide: ActiveGameService, useValue: activeGameService },
        { provide: ActiveGameConnection, useValue: activeGameConnection },
      ],
    });
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  // ------------------------------------------------------------------------
  // connection
  // ------------------------------------------------------------------------

  test.for<ConnectionStatus>(['Connected', 'Connecting', 'Disconnected'])(
    'when connectionStatus$ emits %s then sets connectionStatus with emitted value',
    (status: ConnectionStatus) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      // when
      activeGameConnection.connectionStatus$.next(status);

      // then
      expect(store.connectionStatus()).toEqual(status);
    },
  );

  test('when connect then calls activeGameConnection.connect', async () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    await store.connect();

    // then
    expect(activeGameConnection.connect).toHaveBeenCalledOnce();
  });

  test('when disconnect then calls activeGameConnection.disconnect', async () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    await store.disconnect();

    // then
    expect(activeGameConnection.disconnect).toHaveBeenCalledOnce();
  });

  // ------------------------------------------------------------------------
  // players leaving
  // ------------------------------------------------------------------------

  test.for<Game>(GAME_TESTCASES.filter((game) => game.players.some((player) => !player.isMe)))(
    'given game $id when leftPlayers$ emits not me then removes player from game',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));

      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });

      const leavingPlayer = game.players.find((player) => !player.isMe)!;

      // when
      activeGameConnection.leftPlayers$.next(leavingPlayer);

      // then
      expect(store.game()?.players).not.toContain(leavingPlayer);
    },
  );

  test.for<Game>(GAME_TESTCASES.filter((game) => game.players.some((player) => player.isMe)))(
    'given game $id when leftPlayers$ emits me then resets state',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));

      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });

      const me = game.players.find((player) => player.isMe)!;

      // when
      activeGameConnection.leftPlayers$.next(me);

      // then
      expect(store.game()).toBeNull();
      expect(store.loadingStatus()).toEqual('initial');
    },
  );

  test.for<Player>(PLAYER_TESTCASES)(
    'given no active game when leftPlayers$ emits then game is still null',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      // when
      activeGameConnection.leftPlayers$.next(player);

      // then
      expect(store.game()).toBeNull();
    },
  );

  // ------------------------------------------------------------------------
  // turn completed
  // ------------------------------------------------------------------------

  test.for<Game>(GAME_TESTCASES)(
    'given game $id when turnCompleted$ emits then the received player is on turn in the place tile phase',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);
      const nextPlayer = game.players.at(-1)!;

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });

      // when
      activeGameConnection.turnCompleted$.next(nextPlayer);

      // then
      expect(store.game()?.currentTurnPosition).toEqual(nextPlayer.turnPosition);
      expect(store.turnPhase()).toEqual('PlaceTile');
      expect(store.currentPlayer()).toEqual(nextPlayer);
    },
  );

  test.for<Player>(PLAYER_TESTCASES)(
    'given no active game when turnCompleted$ emits $name then game is still null',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      // when
      activeGameConnection.turnCompleted$.next(player);

      // then
      expect(store.game()).toBeNull();
    },
  );

  // ------------------------------------------------------------------------
  // game completed
  // ------------------------------------------------------------------------

  test.for<Game>(GAME_TESTCASES)(
    'given game $id when gameCompleted$ emits then resets the game state',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // when
      activeGameConnection.gameCompleted$.next();
      TestBed.tick();

      // then
      expect(store.game()).toBeNull();
      expect(store.loadingStatus()).toEqual('initial');
      expect(store.drawnTile()).toBeNull();
      expect(store.drawnTileRotation()).toEqual(0);
      expect(store.isCompleted()).toBe(true);
    },
  );

  test('given new store then isCompleted is false', () => {
    // given & when
    const store = TestBed.inject(ActiveGameStore);

    // then
    expect(store.isCompleted()).toBe(false);
  });

  test('given completed game when loadActiveGame then isCompleted is false again', () => {
    // given
    const store = TestBed.inject(ActiveGameStore);
    activeGameConnection.gameCompleted$.next();

    activeGameService.getActiveGame.mockReturnValueOnce(of(DEFAULT_TEST_GAME));

    // when
    TestBed.runInInjectionContext(() => {
      store.loadActiveGame();
    });

    // then
    expect(store.isCompleted()).toBe(false);
  });

  // ------------------------------------------------------------------------
  // loading the active game
  // ------------------------------------------------------------------------

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

  // ------------------------------------------------------------------------
  // rotating the drawn tile
  // ------------------------------------------------------------------------

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnToPlaceTile))(
    'given drawn tile when drawn then rotation is 0 and rotatedDrawnTile equals the drawn tile',
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
      expect(store.drawnTileRotation()).toEqual(0);
      expect(store.rotatedDrawnTile()).toEqual(store.drawnTile());
    },
  );

  test.for([
    { steps: [1], expected: 1 },
    { steps: [-1], expected: 5 },
    { steps: [1, 1, 1], expected: 3 },
    { steps: [1, -1], expected: 0 },
    { steps: [1, 1, 1, 1, 1, 1], expected: 0 },
  ])(
    'given drawn tile when rotateDrawnTile with $steps then rotation is $expected',
    ({ steps, expected }) => {
      // given
      const store = TestBed.inject(ActiveGameStore);
      const tile = DEFAULT_TEST_GAME.tiles[0];

      activeGameService.getActiveGame.mockReturnValueOnce(
        of(GAME_TESTCASES.filter(isMyTurnToPlaceTile)[0]),
      );
      activeGameService.getNextTile.mockReturnValueOnce(of(ok(tile)));

      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // when
      steps.forEach((step) => store.rotateDrawnTile(step));

      // then
      expect(store.drawnTileRotation()).toEqual(expected);
    },
  );

  test('given no drawn tile when rotateDrawnTile then rotation stays 0', () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    store.rotateDrawnTile(1);

    // then
    expect(store.drawnTileRotation()).toEqual(0);
    expect(store.rotatedDrawnTile()).toBeNull();
  });
});
