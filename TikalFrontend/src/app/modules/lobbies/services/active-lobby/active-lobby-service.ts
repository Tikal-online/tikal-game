import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { catchError, map, Observable, of, Subject, throwError } from 'rxjs';
import { Lobby, markAuthenticatedPlayer } from '../../models/lobby';
import { ConnectionStatus } from '../../../../core/enums/connection-status';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../../../../environments/environment';
import { Player } from '../../models/player';
import { ChatMessage } from '../../models/chat-message';
import { ChatMessageDto } from '../../../../core/dtos/chat-message';
import { AccountStore } from '../../../../core/stores/account-store/account-store';

@Service()
export class ActiveLobbyService {
  readonly message$ = new Subject<ChatMessage>();

  readonly joinedPlayer$ = new Subject<Player>();

  readonly leftPlayers$ = new Subject<Player>();

  readonly updatedPlayers$ = new Subject<Player>();

  readonly connectionStatus$ = new Subject<ConnectionStatus>();

  private readonly accountStore = inject(AccountStore);

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

    this.connection.on('ReceiveMessage', (message: ChatMessageDto) => {
      this.message$.next({ ...message, time: new Date() });
    });

    this.connection.on('PlayerJoined', (player: Player) => {
      if (this.accountStore.isMe(player.userId)) {
        player.isMe = true;
      }

      this.joinedPlayer$.next(player);
    });

    this.connection.on('PlayerLeft', (player: Player) => {
      if (this.accountStore.isMe(player.userId)) {
        player.isMe = true;
      }

      this.leftPlayers$.next(player);
    });

    this.connection.on('PlayerUpdated', (player: Player) => {
      if (this.accountStore.isMe(player.userId)) {
        player.isMe = true;
      }

      this.updatedPlayers$.next(player);
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
      map((lobby: Lobby) => markAuthenticatedPlayer(lobby, this.accountStore.account()?.userId)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 404) {
          return of(null);
        }

        return throwError(() => error);
      }),
    );
  }

  leaveLobby(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}/Players/me`);
  }

  readyUp(): Observable<void> {
    return this.http.put<void>('/Api/Players/me/ready', {});
  }

  readyDown(): Observable<void> {
    return this.http.delete<void>('/Api/Players/me/ready');
  }

  sendMessage(id: number, message: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${id}/Messages`, { message: message });
  }
}
