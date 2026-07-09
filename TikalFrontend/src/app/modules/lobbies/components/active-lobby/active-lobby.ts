import { Component, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/lobby/active-lobby-store';
import { LobbyPlayerListHeader } from '../lobby-player-list-header/lobby-player-list-header';
import { LobbyPlayerList } from '../lobby-player-list/lobby-player-list';
import { Router } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { LobbyNotFoundOverlay } from '../error-overlays/lobby-not-found-overlay/lobby-not-found-overlay';

@Component({
  selector: 'app-active-lobby',
  imports: [LobbyPlayerListHeader, LobbyPlayerList, TranslocoDirective, LobbyNotFoundOverlay],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobby {
  private readonly router = inject(Router);

  readonly activeLobbyStore = inject(ActiveLobbyStore);

  constructor() {
    this.activeLobbyStore.loadActiveLobby();
  }

  leaveLobby(): void {
    this.activeLobbyStore.leaveLobby();

    this.router.navigate(['/lobbies']);
  }
}
