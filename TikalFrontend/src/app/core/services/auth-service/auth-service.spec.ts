import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService, Session } from './auth-service';
import { TestBed } from '@angular/core/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import { ERROR_RESPONSES, HttpResponseData, UNAUTHORIZED } from '../../tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';

const DEFAULT_SESSION: Session = [];

describe('AuthService', () => {
  // dependencies
  let http: HttpTestingController;

  // under test
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthService, provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(AuthService);
  });

  test('getSession returns Unauthorized when GET /bff/user returns 401', async () => {
    const promise = firstValueFrom(service.getSession());

    const req = http.expectOne({ method: 'GET', url: '/bff/user' });
    req.flush('', UNAUTHORIZED);

    const result = await promise;

    expect(result.isErr()).toBeTruthy();
    if (result.isErr()) {
      expect(result.error).toEqual({ type: 'Unauthorized' });
    }
  });

  test('getSession returns Session when GET /bff/user returns Success', async () => {
    const promise = firstValueFrom(service.getSession());

    const req = http.expectOne({ method: 'GET', url: '/bff/user' });
    req.flush(DEFAULT_SESSION);

    const result = await promise;

    expect(result.isOk()).toBeTruthy();
    if (result.isOk()) {
      expect(result.value).toEqual(DEFAULT_SESSION);
    }
  });

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status !== 401))(
    'getSession throws error when GET /bff/user returns $status',
    async (error: HttpResponseData) => {
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.getSession().pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      const req = http.expectOne({ method: 'GET', url: '/bff/user' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );

  afterEach(() => {
    http.verify();
  });
});
