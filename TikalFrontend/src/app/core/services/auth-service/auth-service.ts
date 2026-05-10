import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { err, ok, Result } from 'neverthrow';
import { catchError, map, Observable, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';

export type Claim = {
  type: string;
  value: string;
};

export type Session = Claim[];

export type Unauthorized = {
  type: 'Unauthorized';
};

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  GetSession(): Observable<Result<Session, Unauthorized>> {
    return this.http.get<Session>(`${environment.backend_url}/bff/user`).pipe(
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
