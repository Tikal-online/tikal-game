import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActiveGameService } from './active-game-service';
import { TestBed } from '@angular/core/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import { DEFAULT_TEST_GAME, GAME_TESTCASES } from '../../tests/data/game-test-cases';
import { Game } from '../../models/game';
import { Tile } from '../../models/tile';
import {
  ERROR_RESPONSES,
  HttpResponseData,
  NOT_FOUND,
  CONFLICT,
} from '../../../../core/tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';
import { signal } from '@angular/core';
import { AccountStore } from '../../../../core/stores/account-store/account-store';
import { Account } from '../../../../core/services/account-service/account-service';

describe('ActiveGameService', () => {
  // dependencies
  let http: HttpTestingController;
  const account = signal<Account | null>(null);

  // under test
  let service: ActiveGameService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ActiveGameService,
        provideHttpClientTesting(),
        { provide: AccountStore, useValue: { account } },
      ],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(ActiveGameService);
  });

  afterEach(() => {
    http.verify();
    account.set(null);
  });

  test('getActiveGame returns game when GET /Api/Games/me returns success', async () => {
    // given
    const promise = firstValueFrom(service.getActiveGame());

    // when & then
    const req = http.expectOne({ method: 'GET', url: '/Api/Games/me' });
    req.flush(DEFAULT_TEST_GAME);

    expect(await promise).toEqual(DEFAULT_TEST_GAME);
  });

  test.for<Game>(GAME_TESTCASES)(
    'getActiveGame marks the authenticated player with isMe in game $id when GET /Api/Games/me returns success',
    async (testGame: Game) => {
      // given
      const me = testGame.players.at(-1)!;
      account.set({ userId: me.userId, name: me.name });

      const promise = firstValueFrom(service.getActiveGame());

      // when & then
      const req = http.expectOne({ method: 'GET', url: '/Api/Games/me' });
      req.flush(testGame);

      const game = await promise;

      expect(game?.players.filter((player) => player.isMe).map((player) => player.userId)).toEqual([
        me.userId,
      ]);
    },
  );

  test('getActiveGame returns null when GET /Api/Games/me returns NotFound', async () => {
    // given
    const promise = firstValueFrom(service.getActiveGame());

    // when & then
    const req = http.expectOne({ method: 'GET', url: '/Api/Games/me' });
    req.flush('', NOT_FOUND);

    expect(await promise).toBeNull();
  });

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status !== 404))(
    'getActiveGame throws error when GET /Api/Games/me returns $status',
    async (error: HttpResponseData) => {
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.getActiveGame().pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      // when & then
      const req = http.expectOne({ method: 'GET', url: '/Api/Games/me' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );

  test.for<Tile>(DEFAULT_TEST_GAME.tiles)(
    'getNextTile returns $type tile when GET /Api/Games/{id}/Tiles/next returns success',
    async (tile: Tile) => {
      // given
      const promise = firstValueFrom(service.getNextTile(DEFAULT_TEST_GAME.id));

      // when & then
      const req = http.expectOne({
        method: 'GET',
        url: `/Api/Games/${DEFAULT_TEST_GAME.id}/Tiles/next`,
      });
      req.flush(tile);

      const result = await promise;

      expect(result.isOk() && result.value).toEqual(tile);
    },
  );

  test.for([
    { response: NOT_FOUND, title: 'Game not found', expected: 'GameNotFound' },
    { response: NOT_FOUND, title: 'No tile left', expected: 'NoTilesLeft' },
    { response: CONFLICT, title: 'Invalid turn phase', expected: 'InvalidTurnPhase' },
  ])(
    'getNextTile returns $expected when GET /Api/Games/{id}/Tiles/next returns $response.status $title',
    async ({ response, title, expected }) => {
      // given
      const promise = firstValueFrom(service.getNextTile(DEFAULT_TEST_GAME.id));

      // when & then
      const req = http.expectOne({
        method: 'GET',
        url: `/Api/Games/${DEFAULT_TEST_GAME.id}/Tiles/next`,
      });
      req.flush({ title }, response);

      const result = await promise;

      expect(result.isErr() && result.error.type).toEqual(expected);
    },
  );

  test.for<HttpResponseData>(
    ERROR_RESPONSES.filter((error) => error.status !== 404 && error.status !== 409),
  )(
    'getNextTile throws error when GET /Api/Games/{id}/Tiles/next returns $status',
    async (error: HttpResponseData) => {
      // given
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.getNextTile(DEFAULT_TEST_GAME.id).pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      // when & then
      const req = http.expectOne({
        method: 'GET',
        url: `/Api/Games/${DEFAULT_TEST_GAME.id}/Tiles/next`,
      });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );
});
