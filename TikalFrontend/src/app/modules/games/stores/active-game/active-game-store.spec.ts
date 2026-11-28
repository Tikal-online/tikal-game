import { TestBed } from '@angular/core/testing';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { ActiveGameStore } from './active-game-store';
import { ActiveGameConnection } from '../../services/active-game/active-game-connection';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { Player } from '../../models/player';
import { Tile } from '../../models/tile';
import { of, Subject, throwError } from 'rxjs';
import { Game } from '../../models/game';
import { DEFAULT_TEST_GAME, GAME_TESTCASES } from '../../tests/data/game-test-cases';
import { err, ok } from 'neverthrow';
import { rotateCosts } from '../../models/tile';
import { PLAYER_TESTCASES } from '../../tests/data/player-test-cases';

/** Is the authenticated player on turn and has to place a tile? */
function isMyTurnToPlaceTile(game: Game): boolean {
  const currentPlayer = game.players.find(
    (player) => player.turnPosition === game.currentTurnPosition,
  );

  return !!currentPlayer?.isMe && game.turnPhase === 'PlaceTile';
}

/** Is the authenticated player on turn and has placed the tile, so the turn can be ended? */
function isMyTurnInActionPhase(game: Game): boolean {
  const currentPlayer = game.players.find(
    (player) => player.turnPosition === game.currentTurnPosition,
  );

  return !!currentPlayer?.isMe && game.turnPhase === 'Actions';
}

describe('ActiveGameStore', () => {
  // dependencies
  const activeGameService = {
    getActiveGame: vi.fn(),
    getNextTile: vi.fn(),
    placeTile: vi.fn(),
    endTurn: vi.fn(),
  };

  const activeGameConnection = {
    connect: vi.fn(),
    disconnect: vi.fn(),
    connectionStatus$: new Subject<ConnectionStatus>(),
    leftPlayers$: new Subject<Player>(),
    turnCompleted$: new Subject<Player>(),
    tilePlaced$: new Subject<Tile>(),
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
  // placing the drawn tile
  // ------------------------------------------------------------------------

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnToPlaceTile))(
    'given my drawn tile in game $id when placeDrawnTile next to a placed tile then sends the place and its rotation',
    async (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);
      // (1, -2) needs a placed tile next to it, (1, -1) is right below
      const gameWithNeighbour: Game = {
        ...game,
        tiles: [...game.tiles, { ...DEFAULT_TEST_GAME.tiles[0], coordinate: { q: 1, r: -1 } }],
      };

      activeGameService.getActiveGame.mockReturnValueOnce(of(gameWithNeighbour));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();
      store.rotateDrawnTile(2);

      activeGameService.placeTile.mockResolvedValueOnce(ok());

      // when
      await store.placeDrawnTile({ q: 1, r: -2 });

      // then
      expect(activeGameService.placeTile).toHaveBeenCalledExactlyOnceWith(
        game.id,
        { q: 1, r: -2 },
        2,
      );
    },
  );

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnToPlaceTile))(
    'given the backend accepts the tile in game $id when placeDrawnTile then puts the turned tile on the board right away',
    async (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);
      // (1, -2) needs a placed tile next to it, (1, -1) is right below
      const gameWithNeighbour: Game = {
        ...game,
        tiles: [...game.tiles, { ...DEFAULT_TEST_GAME.tiles[0], coordinate: { q: 1, r: -1 } }],
      };

      activeGameService.getActiveGame.mockReturnValueOnce(of(gameWithNeighbour));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();
      store.rotateDrawnTile(2);
      const drawnTile = store.drawnTile()!;

      activeGameService.placeTile.mockResolvedValueOnce(ok());

      // when
      await store.placeDrawnTile({ q: 1, r: -2 });

      // then
      expect(store.game()?.tiles).toEqual([
        ...gameWithNeighbour.tiles,
        { ...drawnTile, costs: rotateCosts(drawnTile.costs, 2), coordinate: { q: 1, r: -2 } },
      ]);
      expect(store.turnPhase()).toEqual('Actions');
      expect(store.drawnTile()).toBeNull();
      expect(store.drawnTileRotation()).toEqual(0);
    },
  );

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnToPlaceTile))(
    'given the backend rejects the tile when placeDrawnTile then leaves the board and drawn tile unchanged',
    async (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();
      const drawnTile = store.drawnTile();

      activeGameService.placeTile.mockResolvedValueOnce(err(null));

      // when
      await store.placeDrawnTile({ q: 1, r: -2 });

      // then
      expect(store.game()?.tiles).toEqual(game.tiles);
      expect(store.turnPhase()).toEqual('PlaceTile');
      expect(store.drawnTile()).toEqual(drawnTile);
    },
  );

  test('given no active game when placeDrawnTile then does not call placeTile', async () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    await store.placeDrawnTile({ q: 1, r: -2 });

    // then
    expect(activeGameService.placeTile).not.toHaveBeenCalled();
    expect(store.game()).toBeNull();
  });

  test.for<Game>(GAME_TESTCASES.filter((game) => !isMyTurnToPlaceTile(game)))(
    'given no drawn tile in game $id when placeDrawnTile then does not call placeTile and leaves the game unchanged',
    async (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // when
      await store.placeDrawnTile({ q: 1, r: -2 });

      // then
      expect(activeGameService.placeTile).not.toHaveBeenCalled();
      expect(store.game()).toEqual(game);
    },
  );

  // ------------------------------------------------------------------------
  // ending the turn
  // ------------------------------------------------------------------------

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnInActionPhase))(
    'given my turn in the action phase of game $id when canEndTurn then is true',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // when
      const canEndTurn = store.canEndTurn();

      // then
      expect(canEndTurn).toBe(true);
    },
  );

  test.for<Game>(GAME_TESTCASES.filter((game) => !isMyTurnInActionPhase(game)))(
    'given not my turn or not the action phase in game $id when canEndTurn then is false',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // when
      const canEndTurn = store.canEndTurn();

      // then
      expect(canEndTurn).toBe(false);
    },
  );

  test('given no active game when canEndTurn then is false', () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    const canEndTurn = store.canEndTurn();

    // then
    expect(canEndTurn).toBe(false);
  });

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnInActionPhase))(
    'given my turn in the action phase of game $id when endTurn then ends the turn of the game',
    async (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      activeGameService.endTurn.mockResolvedValueOnce(ok());
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // when
      await store.endTurn();

      // then
      expect(activeGameService.endTurn).toHaveBeenCalledExactlyOnceWith(game.id);
    },
  );

  test.for<Game>(GAME_TESTCASES.filter((game) => !isMyTurnInActionPhase(game)))(
    'given not my turn or not the action phase in game $id when endTurn then does not end the turn',
    async (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      // when
      await store.endTurn();

      // then
      expect(activeGameService.endTurn).not.toHaveBeenCalled();
    },
  );

  test('given no active game when endTurn then does not end a turn', async () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    await store.endTurn();

    // then
    expect(activeGameService.endTurn).not.toHaveBeenCalled();
  });

  // ------------------------------------------------------------------------
  // tile placed
  // ------------------------------------------------------------------------

  const PLACED_TILE: Tile = {
    type: 'temple',
    templeLevel: 2,
    costs: { north: 0, northEast: 1, southEast: 0, south: 2, southWest: 0, northWest: 0 },
    coordinate: { q: 4, r: -3 },
  };

  test.for<Game>(GAME_TESTCASES)(
    'given game $id when tilePlaced$ emits then adds the tile to the board and starts the action phase',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of({ ...game, turnPhase: 'PlaceTile' }));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });

      // when
      activeGameConnection.tilePlaced$.next(PLACED_TILE);

      // then
      expect(store.game()?.tiles).toEqual([...game.tiles, PLACED_TILE]);
      expect(store.turnPhase()).toEqual('Actions');
    },
  );

  test.for<Game>(GAME_TESTCASES.filter(isMyTurnToPlaceTile))(
    'given my drawn tile in game $id when tilePlaced$ emits then clears the drawn tile and its rotation',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);

      activeGameService.getActiveGame.mockReturnValueOnce(of(game));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();
      store.rotateDrawnTile(2);

      // when
      activeGameConnection.tilePlaced$.next(PLACED_TILE);

      // then
      expect(store.drawnTile()).toBeNull();
      expect(store.drawnTileRotation()).toEqual(0);
    },
  );

  test.for<Game>(GAME_TESTCASES)(
    'given game $id with a tile at the coordinate when tilePlaced$ emits then leaves the game unchanged',
    (game: Game) => {
      // given
      const store = TestBed.inject(ActiveGameStore);
      const existingTile: Tile = { ...PLACED_TILE, type: 'empty' };
      const gameWithTile: Game = {
        ...game,
        turnPhase: 'PlaceTile',
        tiles: [...game.tiles, existingTile],
      };

      activeGameService.getActiveGame.mockReturnValueOnce(of(gameWithTile));
      TestBed.runInInjectionContext(() => {
        store.loadActiveGame();
      });
      TestBed.tick();

      const drawnTileBefore = store.drawnTile();

      // when
      activeGameConnection.tilePlaced$.next(PLACED_TILE);

      // then
      expect(store.game()).toEqual(gameWithTile);
      expect(store.drawnTile()).toEqual(drawnTileBefore);
    },
  );

  test('given no active game when tilePlaced$ emits then game is still null', () => {
    // given
    const store = TestBed.inject(ActiveGameStore);

    // when
    activeGameConnection.tilePlaced$.next(PLACED_TILE);

    // then
    expect(store.game()).toBeNull();
  });

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
