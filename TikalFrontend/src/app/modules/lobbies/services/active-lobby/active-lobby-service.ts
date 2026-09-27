import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { Lobby } from '../../models/lobby';
import { err, ok, Result } from 'neverthrow';
import { LobbyInGame, LobbyNotFound, PlayerNotInLobby } from '../../errors/lobby-errors';

@Service()
export class ActiveLobbyService {
  private readonly url = '/Api/Lobbies';

  private readonly http = inject(HttpClient);

  getActiveLobby(): Observable<Lobby | null> {
    return this.http.get<Lobby>(this.url + `/me`).pipe(
      map((lobby: Lobby) => lobby),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  leaveLobby(id: number): Observable<Result<void, LobbyNotFound | PlayerNotInLobby | LobbyInGame>> {
    return this.http.delete<void>(`${this.url}/${id}/Players/me`).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          if (error.error.title === 'Lobby not found') {
            return err({ type: 'LobbyNotFound' } as const);
          } else {
            return err({ type: 'PlayerNotInLobby' } as const);
          }
        } else if (error.status === 409) {
          return err({ type: 'LobbyInGame' } as const);
        }

        return throwError(() => error);
      }),
    );
  }
}
