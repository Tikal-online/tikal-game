import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { LobbyService, LobbySummary } from './lobby-service';
import { TestBed } from '@angular/core/testing';
import { PaginatedResult } from '../../../../core/dtos/paginated-result';
import { catchError, firstValueFrom, of } from 'rxjs';
import { Lobby } from '../../models/lobby';
import {
  ERROR_RESPONSES,
  HttpResponseData,
  NOT_FOUND,
} from '../../../../core/tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';

const DEFAULT_PAGINATED_RESULT: PaginatedResult<LobbySummary[]> = {
  data: [],
  totalCount: 0,
};

const DEFAULT_LOBBY: Lobby = {
  id: 1,
  name: 'name',
  maxPlayers: 4,
  players: [],
};

describe('LobbyService', () => {
  // dependencies
  let http: HttpTestingController;

  // under test
  let service: LobbyService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LobbyService, provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(LobbyService);
  });

  afterEach(() => {
    http.verify();
  });

  test.for<[number, number, string]>([
    [1, 1, ''],
    [10, 1, 'searchText'],
    [234234234, 349823424, 'asldkfja342342!??'],
  ])(
    'getLobbiesSummary calls GET /Api/Lobbies with pageSize %i, pageNumber %i and searchText %s',
    async ([pageSize, pageNumber, searchText]) => {
      // given
      const promise = firstValueFrom(service.getLobbiesSummary(pageSize, pageNumber, searchText));

      // when & then
      const req = http.expectOne({
        method: 'GET',
        url: `/Api/Lobbies?pageSize=${pageSize}&pageNumber=${pageNumber}&searchText=${searchText}`,
      });
      req.flush(DEFAULT_PAGINATED_RESULT);

      expect(await promise).toEqual(DEFAULT_PAGINATED_RESULT);
    },
  );

  test.for<number>([1, 2, 353452])(
    'getLobby returns lobby when GET /Api/Lobbies/%i returns Success',
    async (id) => {
      // given
      const promise = firstValueFrom(service.getLobby(id));

      // when & then
      const req = http.expectOne({ method: 'GET', url: `/Api/Lobbies/${id}` });
      req.flush(DEFAULT_LOBBY);

      expect(await promise).toEqual(DEFAULT_LOBBY);
    },
  );

  test.for<number>([1, 2, 353452])(
    'getLobby returns null when GET /Api/Lobbies/%i returns NotFound',
    async (id) => {
      // given
      const promise = firstValueFrom(service.getLobby(id));

      // when & then
      const req = http.expectOne({ method: 'GET', url: `/Api/Lobbies/${id}` });
      req.flush('', NOT_FOUND);

      expect(await promise).toBeNull();
    },
  );

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status !== 404))(
    'getLobby throws error when GET /Api/Lobbies/id returns $status',
    async (error: HttpResponseData) => {
      // given
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.getLobby(1).pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      // when & then
      const req = http.expectOne({ method: 'GET', url: `/Api/Lobbies/${1}` });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );

  test('getActiveLobby returns lobby when GET /Api/Lobbies/me returns success', async () => {
    // given
    const promise = firstValueFrom(service.getActiveLobby());

    // when & then
    const req = http.expectOne({ method: 'GET', url: '/Api/Lobbies/me' });
    req.flush(DEFAULT_LOBBY);

    expect(await promise).toEqual(DEFAULT_LOBBY);
  });

  test('getActiveLobby returns null when GET /Api/Lobbies/me returns NotFound', async () => {
    // given
    const promise = firstValueFrom(service.getActiveLobby());

    // when & then
    const req = http.expectOne({ method: 'GET', url: '/Api/Lobbies/me' });
    req.flush('', NOT_FOUND);

    expect(await promise).toBeNull();
  });

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status !== 404))(
    'getActiveLobby throws error when GET /Api/Lobbies/me returns $status',
    async (error: HttpResponseData) => {
      // given
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.getActiveLobby().pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      // when & then
      const req = http.expectOne({ method: 'GET', url: '/Api/Lobbies/me' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );
});
