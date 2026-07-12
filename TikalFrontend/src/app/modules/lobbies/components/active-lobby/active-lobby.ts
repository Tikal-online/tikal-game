import { Component, DestroyRef, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';
import { LobbyPlayerListHeader } from '../lobby-player-list-header/lobby-player-list-header';
import { LobbyPlayerList } from '../lobby-player-list/lobby-player-list';
import { TranslocoDirective } from '@jsverse/transloco';
import { LobbyNotFoundOverlay } from '../error-overlays/lobby-not-found-overlay/lobby-not-found-overlay';
import { LucideLoaderCircle } from '@lucide/angular';
import { LobbyChat } from '../lobby-chat/lobby-chat';

@Component({
  selector: 'app-active-lobby',
  imports: [
    LobbyPlayerListHeader,
    LobbyPlayerList,
    TranslocoDirective,
    LobbyNotFoundOverlay,
    LucideLoaderCircle,
    LobbyChat,
  ],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobby {
  readonly activeLobbyStore = inject(ActiveLobbyStore);

  constructor() {
    this.activeLobbyStore.loadActiveLobby();
    this.activeLobbyStore.connect();
    inject(DestroyRef).onDestroy(() => this.activeLobbyStore.disconnect());
  }
}
