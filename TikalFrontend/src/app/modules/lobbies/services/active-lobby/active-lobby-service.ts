import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, Subject, throwError } from 'rxjs';
import { Lobby } from '../../models/lobby';
import { ConnectionStatus } from '../../../../core/enums/connection-status';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../../../../environments/environment';
import { Player } from '../../models/player';

@Service()
export class ActiveLobbyService {
  readonly joinedPlayer$ = new Subject<Player>();

  readonly leftPlayers$ = new Subject<Player>();

  readonly connectionStatus$ = new Subject<ConnectionStatus>();

  private readonly url = '/Api/Lobbies';

  private readonly http = inject(HttpClient);

  private readonly connection: HubConnection;

  constructor() {
    this.connection = new HubConnectionBuilder()
      .withUrl(`${environment.backend_url}/Api/hub/activeLobby`, {
        headers: {
          'X-CSRF': '1',
        },
      })
      .build();

    this.connection.on('PlayerJoined', (player: Player) => {
      this.joinedPlayer$.next(player);
    });

    this.connection.on('PlayerLeft', (player: Player) => {
      this.leftPlayers$.next(player);
    });

    this.connection.onclose(() => {
      this.connectionStatus$.next('Disconnected');
    });

    this.connection.onreconnected(() => {
      this.connectionStatus$.next('Connected');
    });

    this.connection.onreconnecting(() => {
      this.connectionStatus$.next('Connecting');
    });
  }

  async connect(): Promise<void> {
    this.connectionStatus$.next('Connecting');
    try {
      await this.connection.start();
      this.connectionStatus$.next('Connected');
    } catch {
      this.connectionStatus$.next('Disconnected');
    }
  }

  disconnect(): Promise<void> {
    return this.connection.stop();
  }

  getActiveLobby(): Observable<Lobby | null> {
    return this.http.get<Lobby>(this.url + '/me').pipe(
      map((lobby: Lobby) => lobby),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  leaveLobby(): Observable<void> {
    return this.http.post<void>(this.url + '/leave', {});
  }
}
