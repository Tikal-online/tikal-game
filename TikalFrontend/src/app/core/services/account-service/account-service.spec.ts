import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Account, AccountService } from './account-service';
import { TestBed } from '@angular/core/testing';
import { catchError, firstValueFrom, of } from 'rxjs';
import { CONFLICT, ERROR_RESPONSES, HttpResponseData, NOT_FOUND } from '../../tests/http-responses';
import { HttpErrorResponse } from '@angular/common/http';

const DEFAULT_ACCOUNT: Account = {
  userId: 'userId',
  name: 'name',
};

describe('AccountService', () => {
  // dependencies
  let http: HttpTestingController;

  // under test
  let service: AccountService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AccountService, provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(AccountService);
  });

  test('getAccount returns null when GET /Api/Accounts/me returns 404', async () => {
    const promise = firstValueFrom(service.getAccount());

    const req = http.expectOne({ method: 'GET', url: '/Api/Accounts/me' });
    req.flush('', NOT_FOUND);

    const result = await promise;

    expect(result).toBeNull();
  });

  test('getAccount returns Account when GET /Api/Accounts/me returns Success', async () => {
    const promise = firstValueFrom(service.getAccount());

    const req = http.expectOne({ method: 'GET', url: '/Api/Accounts/me' });
    req.flush(DEFAULT_ACCOUNT);

    const result = await promise;

    expect(result).toEqual(DEFAULT_ACCOUNT);
  });

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status !== 404))(
    'getAccount throws error when GET /Api/Accounts/me returns $status',
    async (error: HttpResponseData) => {
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.getAccount().pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      const req = http.expectOne({ method: 'GET', url: '/Api/Accounts/me' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );

  test('createAccount returns Conflict when POST /Api/Accounts returns 409', async () => {
    const promise = firstValueFrom(service.createAccount(DEFAULT_ACCOUNT.name));

    const req = http.expectOne({ method: 'POST', url: '/Api/Accounts' });
    req.flush('', CONFLICT);

    const result = await promise;

    expect(result.isErr()).toBeTruthy();
    if (result.isErr()) {
      expect(result.error).toEqual({ type: 'Conflict' });
    }
  });

  test('createAccount returns Account when POST /Api/Accounts returns Success', async () => {
    const promise = firstValueFrom(service.createAccount(DEFAULT_ACCOUNT.name));

    const req = http.expectOne({ method: 'POST', url: '/Api/Accounts' });
    req.flush(DEFAULT_ACCOUNT);

    const result = await promise;

    expect(result.isOk()).toBeTruthy();
    if (result.isOk()) {
      expect(result.value).toEqual(DEFAULT_ACCOUNT);
    }
  });

  test.for<HttpResponseData>(ERROR_RESPONSES.filter((error) => error.status != 409))(
    'createAccount throws error when POST /Api/Accounts returns $status',
    async (error: HttpResponseData) => {
      let capturedError: HttpErrorResponse;

      const promise = firstValueFrom(
        service.createAccount(DEFAULT_ACCOUNT.name).pipe(
          catchError((httpError) => {
            capturedError = httpError;
            return of(httpError);
          }),
        ),
      );

      const req = http.expectOne({ method: 'POST', url: '/Api/Accounts' });
      req.flush('', error);

      await promise;

      expect(capturedError!.status).toEqual(error.status);
    },
  );

  afterEach(() => {
    http.verify();
  });
});
