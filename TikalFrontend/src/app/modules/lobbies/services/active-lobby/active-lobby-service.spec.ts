import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import { Lobby } from '../../models/lobby';
import {
  CONFLICT,
  ERROR_RESPONSES,
  HttpResponseData,
  NOT_FOUND,
  OK,
} from '../../../../core/tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';
import { ActiveLobbyService } from './active-lobby-service';
import { ID_TEST_CASES } from '../../../../core/tests/id-test-cases';

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
  let service: ActiveLobbyService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ActiveLobbyService, provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(ActiveLobbyService);
  });

  afterEach(() => {
    http.verify();
  });

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

  test.for<number>(ID_TEST_CASES)(
    'leaveLobby returns success when DELETE /Api/Lobbies/%i/Players/me returns Success',
    async (id) => {
      // given
      const promise = firstValueFrom(service.leaveLobby(id));

      // when & then
      const req = http.expectOne({ method: 'DELETE', url: `/Api/Lobbies/${id}/Players/me` });
      req.flush(OK);

      const result = await promise;

      expect(result.isOk()).toBeTruthy();
    },
  );

  test.for<number>(ID_TEST_CASES)(
    'leaveLobby returns LobbyNotFound when DELETE /Api/Lobbies/%i/Players/me returns 404 with matching title',
    async (id) => {
      // given
      const promise = firstValueFrom(service.leaveLobby(id));

      // when & then
      const req = http.expectOne({ method: 'DELETE', url: `/Api/Lobbies/${id}/Players/me` });
      req.flush({ title: 'Lobby not found' }, NOT_FOUND);

      const result = await promise;

      expect(result.isErr()).toBeTruthy();
      if (result.isErr()) {
        expect(result.error).toEqual({ type: 'LobbyNotFound' });
      }
    },
  );

  test.for<number>(ID_TEST_CASES)(
    'leaveLobby returns PlayerNotInLobby when DELETE /Api/Lobbies/%i/Players/me returns 404 with matching title',
    async (id) => {
      // given
      const promise = firstValueFrom(service.leaveLobby(id));

      // when & then
      const req = http.expectOne({ method: 'DELETE', url: `/Api/Lobbies/${id}/Players/me` });
      req.flush({ title: 'Player is not in the given lobby' }, NOT_FOUND);

      const result = await promise;

      expect(result.isErr()).toBeTruthy();
      if (result.isErr()) {
        expect(result.error).toEqual({ type: 'PlayerNotInLobby' });
      }
    },
  );

  test.for<number>(ID_TEST_CASES)(
    'leaveLobby returns LobbyInGame when DELETE /Api/Lobbies/%i/Players/me returns 409',
    async (id) => {
      // given
      const promise = firstValueFrom(service.leaveLobby(id));

      // when & then
      const req = http.expectOne({ method: 'DELETE', url: `/Api/Lobbies/${id}/Players/me` });
      req.flush('', CONFLICT);

      const result = await promise;

      expect(result.isErr()).toBeTruthy();
      if (result.isErr()) {
        expect(result.error).toEqual({ type: 'LobbyInGame' });
      }
    },
  );

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => ![404, 409].includes(error.status)))(
    'leaveLobby throws error when GET /Api/Lobbies/id returns $status',
    async (error: HttpResponseData) => {
      // given
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.leaveLobby(1).pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      // when & then
      const req = http.expectOne({ method: 'DELETE', url: `/Api/Lobbies/${1}/Players/me` });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );
});
