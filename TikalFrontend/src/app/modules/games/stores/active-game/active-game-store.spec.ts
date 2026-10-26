import { TestBed } from '@angular/core/testing';
import { ActiveGameService } from '../../services/active-game/active-game-service';
import { ActiveGameStore } from './active-game-store';
import { of, throwError } from 'rxjs';
import { Game } from '../../models/game';
import { GAME_TESTCASES } from '../../tests/data/game-test-cases';

describe('ActiveGameStore', () => {
  // dependencies
  const activeGameService = {
    getActiveGame: vi.fn(),
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [{ provide: ActiveGameService, useValue: activeGameService }],
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
});
