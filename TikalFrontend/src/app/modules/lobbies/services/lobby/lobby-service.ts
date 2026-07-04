import { inject, Service } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { PaginatedResult } from '../../../../core/dtos/paginated-result';
import { err, ok, Result } from 'neverthrow';
import { Lobby } from '../../models/lobby';
import { Conflict } from '../../../../core/dtos/errors';

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

  createLobby(name: string, maxPlayers: number): Observable<Result<Lobby, Conflict>> {
    const body = {
      name: name,
      maxPlayers: maxPlayers,
    };

    return this.http.post<Lobby>(this.url, body).pipe(
      map((lobby: Lobby) => ok(lobby)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 409) {
          return err({ type: 'Conflict' } as const);
        }

        return throwError(() => error);
      }),
    );
  }
}
