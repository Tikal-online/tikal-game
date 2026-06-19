import { Service } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../../../../environments/environment';
import { Subject } from 'rxjs';

export type ConnectionStatus = 'Connected' | 'Connecting' | 'Disconnected';

@Service()
export class GlobalChatService {
  readonly message$ = new Subject<string>();

  readonly connectionStatus$ = new Subject<ConnectionStatus>();

  private readonly connection: HubConnection;

  constructor() {
    const url = `${environment.backend_url}/Api/hub/globalChat`;

    this.connection = new HubConnectionBuilder()
      .withUrl(url, {
        headers: {
          'X-CSRF': '1',
        },
      })
      .build();

    this.connection.on('ReceiveMessage', (message: string) => {
      this.message$.next(message);
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
    await this.connection.start();
    this.connectionStatus$.next('Connected');
  }

  disconnect(): Promise<void> {
    return this.connection.stop();
  }

  sendMessage(message: string): Promise<void> {
    return this.connection.invoke('SendMessage', message);
  }
}
