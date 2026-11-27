import { inject, Service } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../../../../environments/environment';
import { Subject } from 'rxjs';
import { ConnectionStatus } from '../../../../core/types/connection-status';
import { Player } from '../../models/player';
import { Tile } from '../../models/tile';
import { AccountStore } from '../../../../core/stores/account-store/account-store';

@Service({ autoProvided: false })
export class ActiveGameConnection {
  readonly connectionStatus$ = new Subject<ConnectionStatus>();

  readonly leftPlayers$ = new Subject<Player>();

  /** Emits the player whose turn starts now. */
  readonly turnCompleted$ = new Subject<Player>();

  readonly tilePlaced$ = new Subject<Tile>();

  readonly gameCompleted$ = new Subject<void>();

  private readonly connection: HubConnection;

  private readonly accountStore = inject(AccountStore);

  constructor() {
    this.connection = new HubConnectionBuilder()
      .withUrl(`${environment.backend_url}/Api/hub/activeGame`, {
        headers: {
          'X-CSRF': '1',
        },
      })
      .build();

    this.connection.on('PlayerLeft', (player: Omit<Player, 'isMe'>) => {
      this.leftPlayers$.next({ ...player, isMe: this.accountStore.isMe(player.userId) });
    });

    this.connection.on('TurnCompleted', (nextPlayer: Omit<Player, 'isMe'>) => {
      this.turnCompleted$.next({ ...nextPlayer, isMe: this.accountStore.isMe(nextPlayer.userId) });
    });

    this.connection.on('TilePlaced', (tile: Tile) => {
      this.tilePlaced$.next(tile);
    });

    this.connection.on('GameCompleted', () => {
      this.gameCompleted$.next();
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
