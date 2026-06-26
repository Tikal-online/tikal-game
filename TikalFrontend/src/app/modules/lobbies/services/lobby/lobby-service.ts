import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';

export type LobbySummary = {
  id: string;
  name: string;
  maxPlayers: number;
  currentPlayers: number;
};

@Service()
export class LobbyService {
  private readonly http = inject(HttpClient);

  getLobbiesSummary(
    pageSize: number,
    pageNumber: number,
    searchText: string,
  ): Observable<LobbySummary[]> {
    const url = `${environment.backend_url}/Api/Lobbies`;

    return this.http.get<LobbySummary[]>(url, {
      params: {
        pageSize: pageSize,
        pageNumber: pageNumber,
        searchText: searchText,
      },
    });
  }
}
