import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { err, ok, Result } from 'neverthrow';
import { Conflict } from '../../dtos/errors';

export type Account = {
  userId: string;
  name: string;
};

@Service()
export class AccountService {
  private readonly url = '/Api/Accounts';

  private readonly http = inject(HttpClient);

  getAccount(): Observable<Account | null> {
    return this.http.get<Account>(this.url + '/me').pipe(
      map((account: Account) => account),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  createAccount(name: string): Observable<Result<Account, Conflict>> {
    const body = { name: name };

    return this.http.post<Account>(this.url, body).pipe(
      map((account: Account) => ok(account)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 409) {
          return err({ type: 'Conflict' } as const);
        }

        return throwError(() => error);
      }),
    );
  }
}
