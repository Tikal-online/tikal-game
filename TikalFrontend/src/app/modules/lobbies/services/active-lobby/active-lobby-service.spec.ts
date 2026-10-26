import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import {
  CONFLICT,
  CREATED,
  ERROR_RESPONSES,
  HttpResponseData,
  NOT_FOUND,
  OK,
} from '../../../../core/tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';
import { ActiveLobbyService, CreateLobbyData } from './active-lobby-service';
import { ID_TEST_CASES } from '../../../../core/tests/id-test-cases';
import { CREATE_LOBBY_DATA_TEST_CASES } from '../../tests/data/create-lobby-data-test-cases';
import { DEFAULT_TEST_LOBBY } from '../../tests/data/lobby-test-cases';

describe('ActiveLobbyService', () => {
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
    req.flush(DEFAULT_TEST_LOBBY);

    expect(await promise).toEqual(DEFAULT_TEST_LOBBY);
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
    'leaveLobby throws error when DELETE /Api/Lobbies/id/Players/me returns $status',
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

  test.for<CreateLobbyData>(CREATE_LOBBY_DATA_TEST_CASES)(
    'createLobby returns success when POST /Api/Lobbies returns Created',
    async (data) => {
      // given
      const promise = firstValueFrom(service.createLobby(data));

      // when & then
      const req = http.expectOne({ method: 'POST', url: '/Api/Lobbies' });
      req.flush(CREATED);

      const result = await promise;

      expect(result.isOk()).toBeTruthy();
    },
  );

  test.for<CreateLobbyData>(CREATE_LOBBY_DATA_TEST_CASES)(
    'createLobby returns PlayerAlreadyInLobby when POST /Api/Lobbies returns 409',
    async (data) => {
      // given
      const promise = firstValueFrom(service.createLobby(data));

      // when & then
      const req = http.expectOne({ method: 'POST', url: '/Api/Lobbies' });
      req.flush('', CONFLICT);

      const result = await promise;

      expect(result.isErr()).toBeTruthy();
      if (result.isErr()) {
        expect(result.error).toEqual({ type: 'PlayerAlreadyInLobby' });
      }
    },
  );

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status !== 409))(
    'createLobby throws error when POST /Api/Lobbies returns $status',
    async (error: HttpResponseData) => {
      // given
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.createLobby({ name: 'name', maxPlayers: 4 }).pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      // when & then
      const req = http.expectOne({ method: 'POST', url: '/Api/Lobbies' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );

  test.for<number>(ID_TEST_CASES)(
    'startLobby returns success when POST /Api/Lobbies/%i/Start returns Success',
    async (id) => {
      // given
      const promise = firstValueFrom(service.startLobby(id));

      // when & then
      const req = http.expectOne({ method: 'POST', url: `/Api/Lobbies/${id}/Start` });
      req.flush(OK);

      const result = await promise;

      expect(result.isOk()).toBeTruthy();
    },
  );

  test.for<number>(ID_TEST_CASES)(
    'startLobby returns PlayerNotInLobby when POST /Api/Lobbies/%i/Start returns 404',
    async (id) => {
      // given
      const promise = firstValueFrom(service.startLobby(id));

      // when & then
      const req = http.expectOne({ method: 'POST', url: `/Api/Lobbies/${id}/Start` });
      req.flush('', NOT_FOUND);

      const result = await promise;

      expect(result.isErr()).toBeTruthy();
      if (result.isErr()) {
        expect(result.error).toEqual({ type: 'PlayerNotInLobby' });
      }
    },
  );

  test.for<number>(ID_TEST_CASES)(
    'startLobby returns LobbyInGame when POST /Api/Lobbies/%i/Start returns 409',
    async (id) => {
      // given
      const promise = firstValueFrom(service.startLobby(id));

      // when & then
      const req = http.expectOne({ method: 'POST', url: `/Api/Lobbies/${id}/Start` });
      req.flush('', CONFLICT);

      const result = await promise;

      expect(result.isErr()).toBeTruthy();
      if (result.isErr()) {
        expect(result.error).toEqual({ type: 'LobbyInGame' });
      }
    },
  );

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => ![404, 409].includes(error.status)))(
    'startLobby throws error when POST /Api/Lobbies/id/Start returns $status',
    async (error: HttpResponseData) => {
      // given
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.startLobby(1).pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      // when & then
      const req = http.expectOne({ method: 'POST', url: '/Api/Lobbies/1/Start' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );
});
