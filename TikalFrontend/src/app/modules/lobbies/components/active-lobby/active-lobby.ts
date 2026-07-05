import { Component, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/lobby/active-lobby-store';

@Component({
  selector: 'app-active-lobby',
  imports: [],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobby {
  readonly activeLobbyStore = inject(ActiveLobbyStore);
}
