import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { GlobalChatService } from '../../services/global-chat-service/global-chat-service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-lobbies-page',
  imports: [TranslocoDirective],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class LobbiesPage {
  private readonly globalChatService = inject(GlobalChatService);

  async connect(): Promise<void> {
    await this.globalChatService.connect();
  }

  async sendMessage(): Promise<void> {
    await this.globalChatService.sendMessage('Testing');
  }
}
