import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActiveGameService } from './active-game-service';
import { TestBed } from '@angular/core/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import { DEFAULT_TEST_GAME } from '../../tests/data/game-test-cases';
import {
  ERROR_RESPONSES,
  HttpResponseData,
  NOT_FOUND,
} from '../../../../core/tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';

describe('ActiveGameService', () => {
  // dependencies
  let http: HttpTestingController;

  // under test
  let service: ActiveGameService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ActiveGameService, provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(ActiveGameService);
  });

  afterEach(() => {
    http.verify();
  });

  test('getActiveGame returns game when GET /Api/Games/me returns success', async () => {
    // given
    const promise = firstValueFrom(service.getActiveGame());

    // when & then
    const req = http.expectOne({ method: 'GET', url: '/Api/Games/me' });
    req.flush(DEFAULT_TEST_GAME);

    expect(await promise).toEqual(DEFAULT_TEST_GAME);
  });
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
});
