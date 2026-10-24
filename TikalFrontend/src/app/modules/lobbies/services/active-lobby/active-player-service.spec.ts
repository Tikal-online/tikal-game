import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivePlayerService } from './active-player-service';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { NOT_FOUND, OK } from '../../../../core/tests/http-responses';

describe('ActivePlayerService', () => {
  // dependencies
  let http: HttpTestingController;

  // under test
  let service: ActivePlayerService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ActivePlayerService, provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(ActivePlayerService);
  });

  afterEach(() => {
    http.verify();
  });

  test('readyUp returns success when PUT /Api/Players/me returns Success', async () => {
    // given
    const promise = firstValueFrom(service.readyUp());

    // when & then
    const req = http.expectOne({ method: 'PUT', url: '/Api/Players/me/ready' });
    req.flush(OK);

    const result = await promise;

    expect(result.isOk()).toBeTruthy();
  });

  test('readyUp returns PlayerNotInLobby when PUT /Api/Players/me returns 404', async () => {
    // given
    const promise = firstValueFrom(service.readyUp());

    // when & then
    const req = http.expectOne({ method: 'PUT', url: '/Api/Players/me/ready' });
    req.flush('', NOT_FOUND);

    const result = await promise;

    expect(result.isErr()).toBeTruthy();
    if (result.isErr()) {
      expect(result.error).toEqual({ type: 'PlayerNotInLobby' });
    }
  });

  test('readyDown returns success when DELETE /Api/Players/me returns Success', async () => {
    // given
    const promise = firstValueFrom(service.readyDown());

    // when & then
    const req = http.expectOne({ method: 'DELETE', url: '/Api/Players/me/ready' });
    req.flush(OK);

    const result = await promise;

    expect(result.isOk()).toBeTruthy();
  });

  test('readyDown returns PlayerNotInLobby when DELETE /Api/Players/me returns 404', async () => {
    // given
    const promise = firstValueFrom(service.readyDown());

    // when & then
    const req = http.expectOne({ method: 'DELETE', url: '/Api/Players/me/ready' });
    req.flush('', NOT_FOUND);

    const result = await promise;

    expect(result.isErr()).toBeTruthy();
    if (result.isErr()) {
      expect(result.error).toEqual({ type: 'PlayerNotInLobby' });
    }
  });
});
