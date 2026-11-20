import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { Game, markAuthenticatedPlayer } from '../../models/game';
import { Tile } from '../../models/tile';
import { err, ok, Result } from 'neverthrow';
import { GameNotFound, InvalidTurnPhase, NoTilesLeft } from '../../errors/game-errors';
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

  getNextTile(
    gameId: number,
  ): Observable<Result<Tile, GameNotFound | NoTilesLeft | InvalidTurnPhase>> {
    return this.http.get<Tile>(`${this.url}/${gameId}/Tiles/next`).pipe(
      map((tile: Tile) => ok(tile)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          if (error.error?.title === 'No tile left') {
            return of(err({ type: 'NoTilesLeft' } as const));
          }

          return of(err({ type: 'GameNotFound' } as const));
        } else if (error.status === 409) {
          return of(err({ type: 'InvalidTurnPhase' } as const));
        }

        return throwError(() => error);
      }),
    );
  }
}
