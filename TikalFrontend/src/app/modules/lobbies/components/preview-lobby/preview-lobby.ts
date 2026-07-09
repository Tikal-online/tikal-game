import { Component, inject } from '@angular/core';
import { PreviewLobbyStore } from '../../stores/lobby/preview-lobby-store';
import { ActivatedRoute } from '@angular/router';
import { LobbyPlayerListHeader } from '../lobby-player-list-header/lobby-player-list-header';
import { LobbyPlayerList } from '../lobby-player-list/lobby-player-list';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideLoaderCircle } from '@lucide/angular';
import { AlreadyInLobbyOverlay } from '../error-overlays/already-in-lobby-overlay/already-in-lobby-overlay';
import { LobbyNotFoundOverlay } from '../error-overlays/lobby-not-found-overlay/lobby-not-found-overlay';
import { LobbyFullOverlay } from '../error-overlays/lobby-full-overlay/lobby-full-overlay';

@Component({
  selector: 'app-preview-lobby',
  imports: [
    LobbyPlayerListHeader,
    LobbyPlayerList,
    TranslocoDirective,
    LucideLoaderCircle,
    AlreadyInLobbyOverlay,
    LobbyNotFoundOverlay,
    LobbyFullOverlay,
  ],
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
