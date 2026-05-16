import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { err, ok, Result } from 'neverthrow';
import { catchError, map, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';

export type Account = {
  userId: string;
  name: string;
};

export type NotFound = {
  type: 'NotFound';
};

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private readonly http = inject(HttpClient);

  getAccount(): Observable<Result<Account, NotFound>> {
    return this.http.get<Account>(`${environment.backend_url}/Api/Accounts/me`).pipe(
      map((account: Account) => ok(account)),
      catchError((error: HttpErrorResponse) => {
        if (error.status == 404) {
          return err({ type: 'NotFound' } as const);
        }

        return throwError(() => error);
      }),
    );
  }
}
