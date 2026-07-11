import { Observable, of, throwError } from 'rxjs';
import { PaginatedResult } from '../../../../core/dtos/paginated-result';
import { LobbyService, LobbySummary } from '../../services/lobby/lobby-service';
import { TestBed } from '@angular/core/testing';
import { LobbySummaryStore } from './lobby-summary-store';
import { ok } from 'neverthrow';

const DEFAULT_PAGINATED_RESULT: PaginatedResult<LobbySummary[]> = {
  data: [
    { id: 'lobby1Id', name: 'lobbyName', maxPlayers: 3, currentPlayers: 1 },
    { id: 'lobby2Id', name: 'this is my lobby', maxPlayers: 4, currentPlayers: 2 },
  ],
  totalCount: 30,
};

const EMPTY_PAGINATED_RESULT: PaginatedResult<LobbySummary[]> = {
  data: [],
  totalCount: 0,
};

const DEBOUNCETIME = 300;

describe('LobbySummaryStore', () => {
  // dependencies
  const successLobbyService = {
    getLobbiesSummary: (): Observable<PaginatedResult<LobbySummary[]>> =>
      of(DEFAULT_PAGINATED_RESULT),
  };

  const emptyLobbyService = {
    getLobbiesSummary: (): Observable<PaginatedResult<LobbySummary[]>> =>
      of(EMPTY_PAGINATED_RESULT),
  };

  const throwingLobbyService = {
    getLobbiesSummary: (): Observable<PaginatedResult<LobbySummary[]>> => throwError(() => ok()),
  };

  beforeEach(() => {
    vi.useFakeTimers();
  });

  test('loadLobbies resets lobbies and totalCount if lobbies retrieval throws', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: LobbyService, useValue: throwingLobbyService }],
    });

    const store = TestBed.inject(LobbySummaryStore);

    // when
    TestBed.runInInjectionContext(() => {
      store.loadLobbies(of(store.filter()));
    });
    await vi.advanceTimersByTimeAsync(DEBOUNCETIME);

    // then
    expect(store.status()).toEqual('error');
    expect(store.lobbies()).toEqual([]);
    expect(store.totalCount()).toEqual(0);
  });

  test('loadLobbies sets lobbies and totalCount if lobbies retrieval is successfull', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: LobbyService, useValue: successLobbyService }],
    });

    const store = TestBed.inject(LobbySummaryStore);

    // when
    TestBed.runInInjectionContext(() => {
      store.loadLobbies(of(store.filter()));
    });
    await vi.advanceTimersByTimeAsync(DEBOUNCETIME);

    // then
    expect(store.status()).toEqual('loaded');
    expect(store.lobbies()).toEqual(DEFAULT_PAGINATED_RESULT.data);
    expect(store.totalCount()).toEqual(DEFAULT_PAGINATED_RESULT.totalCount);
  });

  test.for<number>([1, 2, 4, 234234])(
    'updatePageNumber with %s sets pageNumber to provided value',
    (value: number) => {
      // given
      TestBed.configureTestingModule({
        providers: [{ provide: LobbyService, useValue: successLobbyService }],
      });

      const store = TestBed.inject(LobbySummaryStore);

      // when
      store.updatePageNumber(value);

      // then
      expect(store.filter.pageNumber()).toEqual(value);
    },
  );

  test.for<string>(['', 'a', 'testing', 'asod90834!2&/'])(
    'updateSearchText with %s sets searchText to provided value and pageNumber to 1',
    (value: string) => {
      // given
      TestBed.configureTestingModule({
        providers: [{ provide: LobbyService, useValue: successLobbyService }],
      });

      const store = TestBed.inject(LobbySummaryStore);

      // when
      store.updateSearchText(value);

      // then
      expect(store.filter.pageNumber()).toEqual(1);
      expect(store.filter.searchText()).toEqual(value);
    },
  );

  test('refresh inverts refreshTrigger', () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: LobbyService, useValue: successLobbyService }],
    });

    const store = TestBed.inject(LobbySummaryStore);

    const refreshValue = store.filter.refreshTrigger();

    // when
    store.refresh();

    // then
    expect(store.filter.refreshTrigger()).toEqual(!refreshValue);
  });

  test.for<number>([2, 3, 10, 234234])(
    'hasPrevious is true if pageNumber is %s',
    (pageNumber: number) => {
      // given
      TestBed.configureTestingModule({
        providers: [{ provide: LobbyService, useValue: successLobbyService }],
      });

      const store = TestBed.inject(LobbySummaryStore);

      // when
      store.updatePageNumber(pageNumber);

      // then
      expect(store.hasPrevious()).toBeTruthy();
    },
  );

  test('noLobbiesFound is true if lobbies retrieval returns empty list', async () => {
    // given
    TestBed.configureTestingModule({
      providers: [{ provide: LobbyService, useValue: emptyLobbyService }],
    });

    const store = TestBed.inject(LobbySummaryStore);

    // when
    TestBed.runInInjectionContext(() => {
      store.loadLobbies(of(store.filter()));
    });
    await vi.advanceTimersByTimeAsync(DEBOUNCETIME);

    // then
    expect(store.noLobbiesFound()).toBeTruthy();
  });
});
