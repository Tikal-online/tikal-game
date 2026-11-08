import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { catchError, firstValueFrom, map, Observable, of, throwError } from 'rxjs';
import { Lobby, markAuthenticatedPlayer } from '../../models/lobby';
import { err, ok, Result } from 'neverthrow';
import {
  LobbyInGame,
  LobbyNotFound,
  PlayerAlreadyInLobby,
  PlayerNotInLobby,
} from '../../errors/lobby-errors';
import { AccountStore } from '../../../../core/stores/account-store/account-store';

export type CreateLobbyData = {
  name: string;
  maxPlayers: number;
};

@Service({ autoProvided: false })
export class ActiveLobbyService {
  private readonly url = '/Api/Lobbies';

  private readonly http = inject(HttpClient);

  private readonly accountStore = inject(AccountStore);

  getActiveLobby(): Observable<Lobby | null> {
    return this.http.get<Lobby>(this.url + `/me`).pipe(
      map((lobby: Lobby) => markAuthenticatedPlayer(lobby, this.accountStore.account()?.userId)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  leaveLobby(id: number): Promise<Result<void, LobbyNotFound | PlayerNotInLobby | LobbyInGame>> {
    const request = this.http.delete<void>(`${this.url}/${id}/Players/me`).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          if (error.error.title === 'Lobby not found') {
            return of(err({ type: 'LobbyNotFound' } as const));
          } else {
            return of(err({ type: 'PlayerNotInLobby' } as const));
          }
        } else if (error.status === 409) {
          return of(err({ type: 'LobbyInGame' } as const));
        }

        return throwError(() => error);
      }),
    );

    return firstValueFrom(request);
  }

  createLobby(data: CreateLobbyData): Promise<Result<void, PlayerAlreadyInLobby>> {
    const request = this.http.post(this.url, data).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 409) {
          return of(err({ type: 'PlayerAlreadyInLobby' } as const));
        }

        return throwError(() => error);
      }),
    );

    return firstValueFrom(request);
  }

  startLobby(id: number): Promise<Result<void, PlayerNotInLobby | LobbyInGame>> {
    const request = this.http.post(`${this.url}/${id}/Start`, {}).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(err({ type: 'PlayerNotInLobby' } as const));
        } else if (error.status === 409) {
          return of(err({ type: 'LobbyInGame' } as const));
        }

        return throwError(() => error);
      }),
    );

    return firstValueFrom(request);
  }
}
