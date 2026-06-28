import { inject, Service } from '@angular/core';
import { map, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { PaginatedResult } from '../../../../core/dtos/paginated-result';

export type LobbySummary = {
  id: string;
  name: string;
  maxPlayers: number;
  currentPlayers: number;
};

@Service()
export class LobbyService {
  private readonly url = '/Api/Lobbies';

  private readonly http = inject(HttpClient);

  getLobbiesSummary(
    pageSize: number,
    pageNumber: number,
    searchText: string,
  ): Observable<PaginatedResult<LobbySummary[]>> {
    return this.http.get<PaginatedResult<LobbySummary[]>>(this.url, {
      params: {
        pageSize: pageSize,
        pageNumber: pageNumber,
        searchText: searchText,
      },
    });
  }
}
