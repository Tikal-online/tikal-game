import { of, Subject, throwError } from 'rxjs';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { Player } from '../../models/player';
import { TestBed } from '@angular/core/testing';
import { ActiveLobbyService } from '../../services/active-lobby/active-lobby-service';
import { ActiveLobbyConnection } from '../../services/active-lobby/active-lobby-connection';
import { ActiveLobbyStore } from './active-lobby-store';
import { Lobby } from '../../models/lobby';
import { DEFAULT_TEST_LOBBY, LOBBY_TESTCASES } from '../../tests/data/lobby-test-cases';
import { PLAYER_TESTCASES } from '../../tests/data/player-test-cases';
import { ActivePlayerService } from '../../services/active-lobby/active-player-service';
import { ok } from 'neverthrow';

describe('ActiveLobbyStore', () => {
  // dependencies
  const activeLobbyService = {
    getActiveLobby: vi.fn(),
    leaveLobby: vi.fn(),
    createLobby: vi.fn(),
    startLobby: vi.fn(),
  };

  const activePlayerService = {
    readyUp: vi.fn(),
    readyDown: vi.fn(),
  };

  const activeLobbyConnection = {
    connect: vi.fn(),
    disconnect: vi.fn(),
    connectionStatus$: new Subject<ConnectionStatus>(),
    joinedPlayers$: new Subject<Player>(),
    updatedPlayers$: new Subject<Player>(),
    leftPlayers$: new Subject<Player>(),
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        { provide: ActiveLobbyService, useValue: activeLobbyService },
        { provide: ActivePlayerService, useValue: activePlayerService },
        { provide: ActiveLobbyConnection, useValue: activeLobbyConnection },
      ],
    });
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  test.for<ConnectionStatus>(['Connected', 'Connecting', 'Disconnected'])(
    'when connectionStatus$ emits %s then sets connectionStatus with emitted value',
    (status: ConnectionStatus) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      // when
      activeLobbyConnection.connectionStatus$.next(status);

      // then
      expect(store.connectionStatus()).toEqual(status);
    },
  );

  test('when connect then calls activeLobbyService.connect', async () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    // when
    await store.connect();

    // then
    expect(activeLobbyConnection.connect).toHaveBeenCalledOnce();
  });

  test('when disconnect then calls activeLobbyService.disconnect', async () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    // when
    await store.disconnect();

    // then
    expect(activeLobbyConnection.disconnect).toHaveBeenCalledOnce();
  });

  test('given no active lobby when loadActiveLobby then sets lobby to null and loadingStatus to loaded', () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    activeLobbyService.getActiveLobby.mockReturnValueOnce(of(null));

    // when
    TestBed.runInInjectionContext(() => {
      store.loadActiveLobby();
    });

    // then
    expect(store.loadingStatus()).toEqual('loaded');
    expect(store.leavingStatus()).toEqual('initial');
    expect(store.creationStatus()).toEqual('initial');
    expect(store.startingStatus()).toEqual('initial');
    expect(store.lobby()).toBeNull();
  });

  test('given error when loadActiveLobby then sets lobby to null and loadingStatus to error', () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    activeLobbyService.getActiveLobby.mockReturnValueOnce(
      throwError(() => new Error('test error')),
    );

    // when
    TestBed.runInInjectionContext(() => {
      store.loadActiveLobby();
    });

    // then
    expect(store.loadingStatus()).toEqual('error');
    expect(store.leavingStatus()).toEqual('initial');
    expect(store.creationStatus()).toEqual('initial');
    expect(store.startingStatus()).toEqual('initial');
    expect(store.lobby()).toBeNull();
  });

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given active lobby when loadActiveLobby then sets lobby to value and loadingStatus to loaded',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // then
      expect(store.loadingStatus()).toEqual('loaded');
      expect(store.leavingStatus()).toEqual('initial');
      expect(store.creationStatus()).toEqual('initial');
      expect(store.startingStatus()).toEqual('initial');
      expect(store.lobby()).toEqual(lobby);
    },
  );

  test('given no active lobby when leaveLobby then does not call leaveLobby', async () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    // when
    TestBed.runInInjectionContext(() => {
      store.leaveActiveLobby();
    });

    // then
    expect(activeLobbyService.leaveLobby).not.toHaveBeenCalled();
  });

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given leaving fails when leaveLobby then sets leavingStatus to error',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.leaveLobby.mockReturnValueOnce(throwError(() => new Error('test error')));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
        store.leaveActiveLobby();
      });

      // then
      expect(store.leavingStatus()).toEqual('error');
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given leaving succeeds when leaveLobby then sets leavingStatus to left and resets state',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.leaveLobby.mockReturnValueOnce(of(null));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
        store.leaveActiveLobby();
      });

      // then
      expect(store.leavingStatus()).toEqual('left');
      expect(store.loadingStatus()).toEqual('initial');
      expect(store.creationStatus()).toEqual('initial');
      expect(store.startingStatus()).toEqual('initial');
      expect(store.lobby()).toBeNull();
    },
  );

  test.for<Player>(PLAYER_TESTCASES)(
    'given active lobby when joinedPlayers$ emits then adds player to lobby',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(DEFAULT_TEST_LOBBY));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when
      activeLobbyConnection.joinedPlayers$.next(player);

      // then
      expect(store.lobby()?.players).toContain(player);
    },
  );

  test.for<Player>(PLAYER_TESTCASES)(
    'given no active lobby when joinedPlayers$ emits then lobby is still null',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      // when
      activeLobbyConnection.joinedPlayers$.next(player);

      // then
      expect(store.lobby()).toBeNull();
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given active lobby when leftPlayers$ emits not me then removes player from lobby',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      const player = lobby.players[0];
      player.isMe = false;

      // when
      activeLobbyConnection.leftPlayers$.next(player);

      // then
      expect(store.lobby()?.players).not.toContain(player);
    },
  );

  test.for<Player>(PLAYER_TESTCASES.filter((p) => !p.isMe))(
    'given no active lobby when leftPlayers$ emits not me then lobby is still null',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      // when
      activeLobbyConnection.leftPlayers$.next(player);

      // then
      expect(store.lobby()).toBeNull();
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given active lobby when leftPlayers$ emits me then sets leavingStatus to left and resets state',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      const player = lobby.players[0];
      player.isMe = true;

      // when
      activeLobbyConnection.leftPlayers$.next(player);

      // then
      expect(store.leavingStatus()).toEqual('left');
      expect(store.loadingStatus()).toEqual('initial');
      expect(store.creationStatus()).toEqual('initial');
      expect(store.startingStatus()).toEqual('initial');
      expect(store.lobby()).toBeNull();
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given active lobby when updatedPlayers$ emits then replaces player with matching userId',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      const player = { ...lobby.players[0], name: 'updatedName' };

      // when
      activeLobbyConnection.updatedPlayers$.next(player);

      // then
      expect(store.lobby()?.players[0]).toEqual(player);
    },
  );

  test.for<Player>(PLAYER_TESTCASES)(
    'given no active lobby when updatedPlayers$ emits then lobby is still null',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      // when
      activeLobbyConnection.updatedPlayers$.next(player);

      // then
      expect(store.lobby()).toBeNull();
    },
  );

  test('when readyUp then calls activePlayerServiceService.readyUp', () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    activePlayerService.readyUp.mockResolvedValueOnce(of(ok()));

    // when
    TestBed.runInInjectionContext(() => {
      store.readyUp();
    });

    // then
    expect(activePlayerService.readyUp).toHaveBeenCalledOnce();
  });

  test('when readyDown then calls activePlayerServiceService.readyDown', () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    activePlayerService.readyDown.mockResolvedValueOnce(of(ok()));

    // when
    TestBed.runInInjectionContext(() => {
      store.readyDown();
    });

    // then
    expect(activePlayerService.readyDown).toHaveBeenCalledOnce();
  });

  test('given no active lobby when startActiveLobby then does not call startLobby', async () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    // when
    TestBed.runInInjectionContext(() => {
      store.startActiveLobby();
    });

    // then
    expect(activeLobbyService.startLobby).not.toHaveBeenCalled();
  });

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given starting fails when startActiveLobby then sets startingStatus to error',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.startLobby.mockReturnValueOnce(throwError(() => new Error('test error')));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
        store.startActiveLobby();
      });

      // then
      expect(store.startingStatus()).toEqual('error');
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given starting succeeds when startActiveLobby then sets startingStatus to started',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.startLobby.mockReturnValueOnce(of(null));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
        store.startActiveLobby();
      });

      // then
      expect(store.startingStatus()).toEqual('started');
    },
  );
});
