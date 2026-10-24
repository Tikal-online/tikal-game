import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { err, ok, Result } from 'neverthrow';
import { catchError, map, Observable, throwError } from 'rxjs';
import { PlayerNotInLobby } from '../../errors/lobby-errors';

@Service()
export class ActivePlayerService {
  private readonly url = '/Api/Players/me';

  private readonly http = inject(HttpClient);

  readyUp(): Observable<Result<void, PlayerNotInLobby>> {
    return this.http.put(`${this.url}/ready`, {}).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return err({ type: 'PlayerNotInLobby' } as const);
        }

        return throwError(() => error);
      }),
    );
  }

  readyDown(): Observable<Result<void, PlayerNotInLobby>> {
    return this.http.delete(`${this.url}/ready`, {}).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return err({ type: 'PlayerNotInLobby' } as const);
        }

        return throwError(() => error);
      }),
    );
  }
}
