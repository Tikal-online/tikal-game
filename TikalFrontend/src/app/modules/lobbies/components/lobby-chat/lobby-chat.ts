import { Component, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';
import { ChatHeader } from '../chat-header/chat-header';
import { LucideMessageSquare } from '@lucide/angular';

@Component({
  selector: 'app-lobby-chat',
  imports: [ChatHeader, LucideMessageSquare],
  templateUrl: './lobby-chat.html',
  styleUrl: './lobby-chat.scss',
})
export class LobbyChat {
  readonly activeLobbyStore = inject(ActiveLobbyStore);
}
