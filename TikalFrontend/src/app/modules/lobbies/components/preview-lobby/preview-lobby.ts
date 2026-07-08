import { Component, inject } from '@angular/core';
import { PreviewLobbyStore } from '../../stores/lobby/preview-lobby-store';
import { LucideArrowLeft } from '@lucide/angular';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LobbyPlayerListHeader } from '../lobby-player-list-header/lobby-player-list-header';
import { LobbyPlayerList } from '../lobby-player-list/lobby-player-list';

@Component({
  selector: 'app-preview-lobby',
  imports: [LucideArrowLeft, RouterLink, LobbyPlayerListHeader, LobbyPlayerList],
  templateUrl: './preview-lobby.html',
  styleUrl: './preview-lobby.scss',
})
export class PreviewLobby {
  private readonly route = inject(ActivatedRoute);

  readonly previewLobbyStore = inject(PreviewLobbyStore);

  constructor() {
    const lobbyId = Number(this.route.snapshot.paramMap.get('id'));

    this.previewLobbyStore.loadLobby(lobbyId);
  }
}
