import { Component, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';

@Component({
  selector: 'tikal-active-lobby',
  imports: [],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobbyComponent {
  readonly activeLobbyStore = inject(ActiveLobbyStore);

  constructor() {
    this.activeLobbyStore.loadActiveLobby();
  }
}
