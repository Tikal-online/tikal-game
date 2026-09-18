import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable, of } from 'rxjs';
import { PaginatedResult } from '../../../../core/dtos/paginated-result';

export type LobbySummary = {
  id: string;
  name: string;
  maxPlayers: number;
  currentPlayers: number;
};

function randomLobby(): LobbySummary {
  const adjectives = [
    'Casual',
    'Ranked',
    'Late Night',
    'Pro',
    'Newbie',
    'Chill',
    'Epic',
    'Quickplay',
  ];
  const nouns = ['Squad', 'Showdown', 'Session', 'Arena', 'Lobby', 'Scrims', 'Hangout'];

  const maxPlayers = [2, 4, 5, 6, 8, 10][Math.floor(Math.random() * 6)];
  const currentPlayers = Math.floor(Math.random() * (maxPlayers + 1)); // 0 to maxPlayers

  return {
    id: crypto.randomUUID(), // or use a package like `uuid` if not in a browser/modern Node env
    name: `${adjectives[Math.floor(Math.random() * adjectives.length)]} ${nouns[Math.floor(Math.random() * nouns.length)]}`,
    maxPlayers,
    currentPlayers,
  };
}

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

  getMockedLobbiesSummary(
    pageSize: number,
    pageNumber: number,
    searchText: string,
  ): Observable<PaginatedResult<LobbySummary[]>> {
    return of({
      data: Array.from({ length: pageSize }, () => randomLobby()),
      totalCount: 351,
    });
  }
}
