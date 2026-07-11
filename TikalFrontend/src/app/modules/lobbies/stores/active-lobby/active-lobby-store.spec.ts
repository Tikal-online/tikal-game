import { of, Subject, throwError } from 'rxjs';
import { ConnectionStatus } from '../../../../core/enums/connection-status';
import { Player } from '../../models/player';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ActiveLobbyService } from '../../services/active-lobby/active-lobby-service';
import { ActiveLobbyStore } from './active-lobby-store';
import { Lobby } from '../../models/lobby';
import { PLAYER_TESTCASES } from '../../models/player.testcases';
import { DEFAULT_TEST_LOBBY, LOBBY_TESTCASES } from '../../models/lobby.testcases';
import { AccountStore } from '../../../../core/stores/account-store/account-store';

describe('ActiveLobbyStore', () => {
  // dependencies
  const router = {
    navigate: vi.fn(),
  };

  const accountStore = {
    isMe: vi.fn(),
  };

  const activeLobbyService = {
    joinedPlayer$: new Subject<Player>(),
    leftPlayers$: new Subject<Player>(),
    connectionStatus$: new Subject<ConnectionStatus>(),
    connect: vi.fn(),
    disconnect: vi.fn(),
    getActiveLobby: vi.fn(),
    leaveLobby: vi.fn(),
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: router },
        { provide: AccountStore, useValue: accountStore },
        { provide: ActiveLobbyService, useValue: activeLobbyService },
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
      activeLobbyService.connectionStatus$.next(status);

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
    expect(activeLobbyService.connect).toHaveBeenCalledOnce();
  });

  test('when disconnect then calls activeLobbyService.disconnect', async () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    // when
    await store.disconnect();

    // then
    expect(activeLobbyService.disconnect).toHaveBeenCalledOnce();
  });

  test('given no active lobby when loadActiveLobby then sets lobby to null and status to loaded', () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    activeLobbyService.getActiveLobby.mockReturnValueOnce(of(null));

    // when
    TestBed.runInInjectionContext(() => {
      store.loadActiveLobby();
    });

    // then
    expect(store.status()).toEqual('loaded');
    expect(store.lobby()).toBeNull();
  });

  test('given error when loadActiveLobby then sets status to error', () => {
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
    expect(store.status()).toEqual('error');
  });

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given active lobby when loadActiveLobby then sets lobby to value and status to loaded',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));

      // when
      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      // then
      expect(store.status()).toEqual('loaded');
      expect(store.lobby()).toEqual(lobby);
    },
  );

  test('given leaving succeeds when leaveLobby then routes back to /lobbies', () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    activeLobbyService.leaveLobby.mockReturnValueOnce(of(undefined));

    // when
    TestBed.runInInjectionContext(() => {
      store.leaveLobby();
    });

    // then
    expect(router.navigate).toHaveBeenCalledWith(['/lobbies']);
  });

  test('given leaving fails when leaveLobby then sets leavingStatus to error', () => {
    // given
    const store = TestBed.inject(ActiveLobbyStore);

    activeLobbyService.leaveLobby.mockReturnValueOnce(throwError(() => new Error('test error')));

    // when
    TestBed.runInInjectionContext(() => {
      store.leaveLobby();
    });

    // then
    expect(store.leavingStatus()).toEqual('error');
  });

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
      activeLobbyService.joinedPlayer$.next(player);

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
      activeLobbyService.joinedPlayer$.next(player);

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
      accountStore.isMe.mockReturnValueOnce(false);

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      const player = lobby.players[0];

      // when
      activeLobbyService.leftPlayers$.next(player);

      // then
      expect(store.lobby()?.players).not.toContain(player);
    },
  );

  test.for<Player>(PLAYER_TESTCASES)(
    'given no active lobby when leftPlayers$ emits not me then lobby is still null',
    (player: Player) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      accountStore.isMe.mockReturnValueOnce(false);

      // when
      activeLobbyService.leftPlayers$.next(player);

      // then
      expect(store.lobby()).toBeNull();
    },
  );

  test.for<Lobby>(LOBBY_TESTCASES)(
    'given active lobby when leftPlayers$ emits me then disconnects and sets status to left',
    (lobby: Lobby) => {
      // given
      const store = TestBed.inject(ActiveLobbyStore);

      activeLobbyService.getActiveLobby.mockReturnValueOnce(of(lobby));
      accountStore.isMe.mockReturnValueOnce(true);

      TestBed.runInInjectionContext(() => {
        store.loadActiveLobby();
      });

      const player = lobby.players[0];

      // when
      activeLobbyService.leftPlayers$.next(player);

      // then
      expect(store.status()).toEqual('left');
      expect(activeLobbyService.disconnect).toHaveBeenCalledOnce();
    },
  );
});
