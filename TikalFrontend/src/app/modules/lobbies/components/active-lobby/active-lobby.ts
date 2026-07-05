import { Component, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/lobby/active-lobby-store';
import { LobbyPlayerListHeader } from '../lobby-player-list-header/lobby-player-list-header';
import { LobbyPlayerList } from '../lobby-player-list/lobby-player-list';

@Component({
  selector: 'app-active-lobby',
  imports: [LobbyPlayerListHeader, LobbyPlayerList],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobby {
  readonly activeLobbyStore = inject(ActiveLobbyStore);

  constructor() {
    this.activeLobbyStore.loadActiveLobby();
  }
}
