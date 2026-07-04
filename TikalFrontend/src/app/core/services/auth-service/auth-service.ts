import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { err, ok, Result } from 'neverthrow';
import { catchError, map, Observable, throwError } from 'rxjs';
import { Unauthorized } from '../../dtos/errors';

export type Claim = {
  type: string;
  value: string;
};

export type Session = Claim[];

@Service()
export class AuthService {
  private readonly url = '/bff/user';

  private readonly http = inject(HttpClient);

  getSession(): Observable<Result<Session, Unauthorized>> {
    return this.http.get<Session>(this.url).pipe(
      map((session: Session) => ok(session)),
      catchError((error: HttpErrorResponse) => {
        if (error.status == 401) {
          return err({ type: 'Unauthorized' } as const);
        }

        return throwError(() => error);
      }),
    );
  }
}
