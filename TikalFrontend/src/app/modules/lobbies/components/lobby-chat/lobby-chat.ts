import { Component, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';
import { ChatHeader } from '../chat-header/chat-header';
import { LucideMessageSquare } from '@lucide/angular';
import { ChatMessages } from '../chat-messages/chat-messages';
import { ChatForm } from '../chat-form/chat-form';
import { ChatConnecting } from '../chat-connecting/chat-connecting';
import { ChatDisconnected } from '../chat-disconnected/chat-disconnected';

@Component({
  selector: 'app-lobby-chat',
  imports: [
    ChatHeader,
    LucideMessageSquare,
    ChatMessages,
    ChatForm,
    ChatConnecting,
    ChatDisconnected,
  ],
  templateUrl: './lobby-chat.html',
  styleUrl: './lobby-chat.scss',
})
export class LobbyChat {
  readonly activeLobbyStore = inject(ActiveLobbyStore);
}
