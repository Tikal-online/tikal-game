import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, throwError } from 'rxjs';

export type Claim = {
  type: string;
  value: string;
};

export type Session = Claim[];

@Service()
export class AuthService {
  private readonly url = '/bff/user';

  private readonly http = inject(HttpClient);

  getSession(): Observable<Session | null> {
    return this.http.get<Session>(this.url).pipe(
      map((session: Session) => session),
      catchError((error: HttpErrorResponse) => {
        if (error.status == 401) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }
}
