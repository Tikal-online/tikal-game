import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { baseUrlInterceptor } from './base-url.interceptor';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';

describe('BaseUrlInterceptor', () => {
  // dependencies
  let http: HttpTestingController;
  let client: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([baseUrlInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    http = TestBed.inject(HttpTestingController);
    client = TestBed.inject(HttpClient);
  });

  test.for<string>(['/i18n/en.json', '/i18n/de.json', 'test/i18n/ajskldfalksdf/en.json'])(
    'does not change request for url %s',
    (url) => {
      firstValueFrom(client.get(url));

      http.expectOne(url);
    },
  );

  test.for<string>(['/Api/Accounts', '/Testing', '/lkasdnasjdf234()§403)=('])(
    'prefixes url %s with configured backend url',
    (url) => {
      firstValueFrom(client.get(url));

      http.expectOne(`${environment.backend_url}${url}`);
    },
  );
});
