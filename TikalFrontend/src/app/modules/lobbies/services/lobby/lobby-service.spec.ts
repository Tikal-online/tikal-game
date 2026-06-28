import { TestBed } from '@angular/core/testing';
import { LobbyService, LobbySummary } from './lobby-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { firstValueFrom } from 'rxjs';
import { PaginatedResult } from '../../../../core/dtos/paginated-result';

const DEFAULT_RESPONSE: PaginatedResult<LobbySummary[]> = {
  data: [],
  totalCount: 0,
};

describe('LobbyService', () => {
  // dependencies
  let http: HttpTestingController;

  // under test
  let service: LobbyService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LobbyService, provideHttpClientTesting()],
    });

    http = TestBed.inject(HttpTestingController);

    service = TestBed.inject(LobbyService);
  });

  test.for<[number, number, string]>([
    [1, 1, ''],
    [10, 1, 'searchText'],
    [234234234, 349823424, 'asldkfja342342!??'],
  ])(
    'calls GET /Api/Lobbies with pageSize %i, pageNumber %i and searchText %s when getLobbiesSummary',
    async ([pageSize, pageNumber, searchText]) => {
      const promise = firstValueFrom(service.getLobbiesSummary(pageSize, pageNumber, searchText));

      const req = http.expectOne({
        method: 'GET',
        url: `/Api/Lobbies?pageSize=${pageSize}&pageNumber=${pageNumber}&searchText=${searchText}`,
      });
      req.flush(DEFAULT_RESPONSE);

      expect(await promise).toEqual(DEFAULT_RESPONSE);
    },
  );

  afterEach(() => {
    http.verify();
  });
});
