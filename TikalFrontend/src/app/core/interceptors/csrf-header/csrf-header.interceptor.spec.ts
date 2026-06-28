import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { csrfHeaderInterceptor } from './csrf-header.interceptor';
import { firstValueFrom } from 'rxjs';

describe('CsrfHeaderInterceptor', () => {
  // dependencies
  let http: HttpTestingController;
  let client: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([csrfHeaderInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    http = TestBed.inject(HttpTestingController);
    client = TestBed.inject(HttpClient);
  });

  test('sets withCredentials and X-CSRF header on every request', () => {
    const url = '/Testing';

    firstValueFrom(client.get(url));

    const req = http.expectOne(url);

    expect(req.request.withCredentials).toBeTruthy();
    expect(req.request.headers.get('X-CSRF')).toBe('1');
  });
});
