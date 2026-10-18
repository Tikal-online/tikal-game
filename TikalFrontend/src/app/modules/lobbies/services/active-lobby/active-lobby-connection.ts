import { inject, Service } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../../../../environments/environment';
import { Subject } from 'rxjs';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { Player } from '../../models/player';
import { AccountStore } from '../../../../core/stores/account-store/account-store';

@Service()
export class ActiveLobbyConnection {
  readonly connectionStatus$ = new Subject<ConnectionStatus>();

  readonly joinedPlayers$ = new Subject<Player>();

  readonly leftPlayers$ = new Subject<Player>();

  readonly updatedPlayers$ = new Subject<Player>();

  private readonly connection: HubConnection;

  private readonly accountStore = inject(AccountStore);

  constructor() {
    this.connection = new HubConnectionBuilder()
      .withUrl(`${environment.backend_url}/Api/hub/activeLobby`, {
        headers: {
          'X-CSRF': '1',
        },
      })
      .build();

    this.connection.on('PlayerJoined', (player: Player) => {
      if (this.accountStore.isMe(player.userId)) {
        player.isMe = true;
      }

      this.joinedPlayers$.next(player);
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
}
