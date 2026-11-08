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
import { err, ok } from 'neverthrow';

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
        ActiveLobbyStore,
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
      expect(store.lobby()).toEqual(lobby);
    },
  );

  test('given no active lobby when leaveActiveLobby then returns PlayerNotInLobby without calling leaveLobby', async () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    // when
    const result = await store.leaveActiveLobby();

    // then
    expect(result).toEqual(err({ type: 'PlayerNotInLobby' }));
    expect(activeLobbyService.leaveLobby).not.toHaveBeenCalled();
  });

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given leaving succeeds when leaveActiveLobby then returns ok',
    async (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.leaveLobby.mockResolvedValueOnce(ok());

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when
      const promise = store.leaveActiveLobby();

      // then
      const result = await promise;

      expect(result.isOk()).toBe(true);
      expect(activeLobbyService.leaveLobby).toHaveBeenCalledWith(lobby.id);
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given leaving returns an expected error when leaveActiveLobby then returns the error',
    async (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.leaveLobby.mockResolvedValueOnce(err({ type: 'LobbyInGame' }));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when
      const result = await store.leaveActiveLobby();

      // then
      expect(result).toEqual(err({ type: 'LobbyInGame' }));
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given leaving fails unexpectedly when leaveActiveLobby then rejects',
    async (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.leaveLobby.mockRejectedValueOnce(new Error('test error'));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when & then
      await expect(store.leaveActiveLobby()).rejects.toThrow('test error');
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
    'given player already in lobby when joinedPlayers$ emits same userId then replaces the player',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);
      const existingPlayer: Player = { ...player, name: 'old name', isReady: !player.isReady };

      activeLobbyService.getActiveLobby.mockReturnValueOnce(
        of({ ...DEFAULT_TEST_LOBBY, players: [existingPlayer] }),
      );

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when
      activeLobbyConnection.joinedPlayers$.next(player);

      // then
      expect(store.lobby()?.players).toEqual([player]);
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
    'given active lobby when leftPlayers$ emits me then resets state',
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
      expect(store.loadingStatus()).toEqual('initial');
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

  test('given no active lobby when startActiveLobby then returns PlayerNotInLobby without calling startLobby', async () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    // when
    const result = await store.startActiveLobby();

    // then
    expect(result).toEqual(err({ type: 'PlayerNotInLobby' }));
    expect(activeLobbyService.startLobby).not.toHaveBeenCalled();
  });

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given starting succeeds when startActiveLobby then returns ok',
    async (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.startLobby.mockResolvedValueOnce(ok());

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when
      const result = await store.startActiveLobby();

      // then
      expect(result.isOk()).toBe(true);
      expect(activeLobbyService.startLobby).toHaveBeenCalledWith(lobby.id);
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given starting returns an expected error when startActiveLobby then returns the error',
    async (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.startLobby.mockResolvedValueOnce(err({ type: 'LobbyInGame' }));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when
      const result = await store.startActiveLobby();

      // then
      expect(result).toEqual(err({ type: 'LobbyInGame' }));
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given starting fails unexpectedly when startActiveLobby then rejects',
    async (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      activeLobbyService.startLobby.mockRejectedValueOnce(new Error('test error'));

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // when & then
      await expect(store.startActiveLobby()).rejects.toThrow('test error');
    },
  );
});
