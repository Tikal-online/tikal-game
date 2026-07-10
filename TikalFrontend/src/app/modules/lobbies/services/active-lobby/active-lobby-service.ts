import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { Lobby } from '../../models/lobby';

@Service()
export class ActiveLobbyService {
  private readonly url = '/Api/Lobbies';

  private readonly http = inject(HttpClient);

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
}
