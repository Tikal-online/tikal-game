import { Service } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../../../../environments/environment';
import { Subject } from 'rxjs';
import { ConnectionStatus } from '../../../../core/enums/connection-status';

type ChatMessageDto = {
  userId: string;
  username: string;
  content: string;
};

export type ChatMessage = {
  userId: string;
  username: string;
  content: string;
  time: Date;
};

// TODO: find a clean way to unit this service
@Service()
export class GlobalChatService {
  readonly message$ = new Subject<ChatMessage>();

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

    this.connection.on('ReceiveMessage', (message: ChatMessageDto) => {
      this.message$.next({ ...message, time: new Date() });
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

  sendMessage(message: string): Promise<void> {
    return this.connection.invoke('SendMessage', message);
  }
}
