import { TestBed } from '@angular/core/testing';
import { LobbyService, LobbySummary } from './lobby-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import { PaginatedResult } from '../../../../core/dtos/paginated-result';
import {
  CONFLICT,
  CREATED,
  ERROR_RESPONSES,
  HttpResponseData,
} from '../../../../core/tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';

const DEFAULT_RESPONSE: PaginatedResult<LobbySummary[]> = {
  data: [],
  totalCount: 0,
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

  test.for<[number, number, string]>([
    [1, 1, ''],
    [10, 1, 'searchText'],
    [234234234, 349823424, 'asldkfja342342!??'],
  ])(
    'getLobbiesSummary calls GET /Api/Lobbies with pageSize %i, pageNumber %i and searchText %s',
    async ([pageSize, pageNumber, searchText]) => {
      const promise = firstValueFrom(service.getLobbiesSummary(pageSize, pageNumber, searchText));

      const req = http.expectOne({
        method: 'GET',
        url: `/Api/Lobbies?pageSize=${pageSize}&pageNumber=${pageNumber}&searchText=${searchText}`,
      });
      req.flush(DEFAULT_RESPONSE);

      expect(await promise).toEqual(DEFAULT_RESPONSE);
    },
  );

  test('createLobby returns Conflict when POST /Api/Lobbies returns 409', async () => {
    const promise = firstValueFrom(service.createLobby('LobbyName', 4));

    const req = http.expectOne({ method: 'POST', url: '/Api/Lobbies' });
    req.flush('', CONFLICT);

    const result = await promise;

    expect(result.isErr()).toBeTruthy();
    if (result.isErr()) {
      expect(result.error).toEqual({ type: 'Conflict' });
    }
  });

  test('createLobby returns Ok when POST /Api/Lobbies returns Success', async () => {
    const promise = firstValueFrom(service.createLobby('LobbyName', 4));

    const req = http.expectOne({ method: 'POST', url: '/Api/Lobbies' });
    req.flush('', CREATED);

    const result = await promise;

    expect(result.isOk()).toBeTruthy();
  });

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status !== 409))(
    'createLobby throws error when POST /Api/Lobbies returns $status',
    async (error: HttpResponseData) => {
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.createLobby('LobbyName', 4).pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      const req = http.expectOne({ method: 'POST', url: '/Api/Lobbies' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );

  afterEach(() => {
    http.verify();
  });
});
