import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
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

export type JoinLobbyError =
  { type: 'PlayerAlreadyInALobby' } | { type: 'LobbyNotFound' } | { type: 'LobbyFull' };

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

  createLobby(name: string, maxPlayers: number): Observable<Result<void, Conflict>> {
    const body = {
      name: name,
      maxPlayers: maxPlayers,
    };

    return this.http.post(this.url, body).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 409) {
          return err({ type: 'Conflict' } as const);
        }

        return throwError(() => error);
      }),
    );
  }

  getLobby(id: number): Observable<Lobby | null> {
    return this.http.get<Lobby>(this.url + `/${id}`).pipe(
      map((lobby: Lobby) => lobby),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  getActiveLobby(): Observable<Lobby | null> {
    return this.http.get<Lobby>(this.url + '/me').pipe(
      map((lobby: Lobby) => lobby),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  leaveLobby(): Observable<void> {
    return this.http.post<void>(this.url + '/leave', {});
  }

  joinLobby(id: number): Observable<Result<void, JoinLobbyError>> {
    return this.http.post<void>(this.url + `/${id}/join`, '').pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        // TODO: improve problem response error handling
        if (error.status === 409) {
          console.log(error.message);
          if (error.error.title === 'Player is already in a lobby') {
            return err({ type: 'PlayerAlreadyInALobby' } as const);
          } else {
            return err({ type: 'LobbyFull' } as const);
          }
        } else if (error.status === 404) {
          return err({ type: 'LobbyNotFound' } as const);
        }

        return throwError(() => error);
      }),
    );
  }
}
