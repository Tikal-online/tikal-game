import { inject, Service } from '@angular/core';
import { catchError, firstValueFrom, map, Observable, of, throwError } from 'rxjs';
import { Game, markAuthenticatedPlayer } from '../../models/game';
import { HexCoordinate, Tile } from '../../models/tile';
import { err, ok, Result } from 'neverthrow';
import {
  GameNotFound,
  InvalidTurnPhase,
  NotCurrentPlayer,
  NoTilesLeft,
  TileCannotBePlaced,
  TileMustBeDrawnFirst,
} from '../../errors/game-errors';
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

  placeTile(
    gameId: number,
    coordinate: HexCoordinate,
    rotation: number,
  ): Promise<
    Result<
      void,
      GameNotFound | NotCurrentPlayer | InvalidTurnPhase | TileMustBeDrawnFirst | TileCannotBePlaced
    >
  > {
    const request = this.http
      .post<void>(`${this.url}/${gameId}/Tiles`, { q: coordinate.q, r: coordinate.r, rotation })
      .pipe(
        map(() => ok()),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 404) {
            return of(err({ type: 'GameNotFound' } as const));
          } else if (error.status === 403) {
            return of(err({ type: 'NotCurrentPlayer' } as const));
          } else if (error.status === 409) {
            if (error.error?.title === 'Tile can not be placed') {
              return of(err({ type: 'TileCannotBePlaced' } as const));
            } else if (error.error?.title === 'Invalid turn phase') {
              return of(err({ type: 'InvalidTurnPhase' } as const));
            }

            return of(err({ type: 'TileMustBeDrawnFirst' } as const));
          }

          return throwError(() => error);
        }),
      );

    return firstValueFrom(request);
  }
}
