import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { Game, markAuthenticatedPlayer } from '../../models/game';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AccountStore } from '../../../../core/stores/account-store/account-store';

@Service({ autoProvided: false })
export class ActiveGameService {
  private readonly url = '/Api/Games';

  private readonly http = inject(HttpClient);

  private readonly accountStore = inject(AccountStore);

  getActiveGame(): Observable<Game | null> {
    return this.http.get<Game>(this.url + '/me').pipe(
      map((game: Game) => markAuthenticatedPlayer(game, this.accountStore.account()?.userId)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }
}
