import { inject, Service } from '@angular/core';
import { AccountStore } from '../../../../core/stores/account-store/account-store';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { Game } from '../../models/game';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Service()
export class ActiveGameService {
  private readonly url = '/Api/Games';

  private readonly http = inject(HttpClient);

  private readonly accountStore = inject(AccountStore);

  getActiveGame(): Observable<Game | null> {
    return this.http.get<Game>(this.url + '/me').pipe(
      map((game: Game) => game),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }
}
