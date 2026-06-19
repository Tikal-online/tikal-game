import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { catchError, map, Observable, throwError } from 'rxjs';
import { err, ok, Result } from 'neverthrow';

export type Account = {
  userId: string;
  name: string;
};

export type NotFound = {
  type: 'NotFound';
};

export type Conflict = {
  type: 'Conflict';
};

@Service()
export class AccountService {
  private readonly http = inject(HttpClient);

  getAccount(): Observable<Result<Account, NotFound>> {
    const url = `${environment.backend_url}/Api/Accounts/me`;

    return this.http.get<Account>(url).pipe(
      map((account: Account) => ok(account)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return err({ type: 'NotFound' } as const);
        }

        return throwError(() => error);
      }),
    );
  }

  createAccount(name: string): Observable<Result<Account, Conflict>> {
    const url = `${environment.backend_url}/Api/Accounts`;
    const body = { name: name };

    return this.http.post<Account>(url, body).pipe(
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
