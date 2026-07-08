import { Component, inject } from '@angular/core';
import { PreviewLobbyStore } from '../../stores/lobby/preview-lobby-store';

@Component({
  selector: 'app-preview-lobby',
  imports: [],
  templateUrl: './preview-lobby.html',
  styleUrl: './preview-lobby.scss',
})
export class PreviewLobby {
  readonly previewLobbyStore = inject(PreviewLobbyStore);
}
