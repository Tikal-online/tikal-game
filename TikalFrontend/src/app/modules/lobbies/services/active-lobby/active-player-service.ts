import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { err, ok, Result } from 'neverthrow';
import { catchError, firstValueFrom, map, throwError } from 'rxjs';
import { PlayerNotInLobby } from '../../errors/lobby-errors';

@Service()
export class ActivePlayerService {
  private readonly url = '/Api/Players/me';

  private readonly http = inject(HttpClient);

  readyUp(): Promise<Result<void, PlayerNotInLobby>> {
    const request = this.http.put(`${this.url}/ready`, {}).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return err({ type: 'PlayerNotInLobby' } as const);
        }

        return throwError(() => error);
      }),
    );

    return firstValueFrom(request);
  }

  readyDown(): Promise<Result<void, PlayerNotInLobby>> {
    const request = this.http.delete(`${this.url}/ready`, {}).pipe(
      map(() => ok()),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return err({ type: 'PlayerNotInLobby' } as const);
        }

        return throwError(() => error);
      }),
    );

    return firstValueFrom(request);
  }
}
