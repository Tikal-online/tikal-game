import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import { Lobby } from '../../models/lobby';
import {
  ERROR_RESPONSES,
  HttpResponseData,
  NOT_FOUND,
} from '../../../../core/tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';
import { ActiveLobbyService } from './active-lobby-service';

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
});
