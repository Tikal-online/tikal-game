import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class GlobalChatService {
  private readonly connection: HubConnection;

  constructor() {
    this.connection = new HubConnectionBuilder()
      .withUrl('https://localhost:7015/Api/hub/globalChat')
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveMessage', (message: string) => {
      console.log(message);
    });
  }

  connect(): Promise<void> {
    return this.connection.start();
  }

  sendMessage(message: string): Promise<void> {
    return this.connection.invoke('SendMessage', message);
  }
}
