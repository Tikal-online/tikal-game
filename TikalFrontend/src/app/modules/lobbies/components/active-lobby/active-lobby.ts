import { Component, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';
import { TranslocoDirective } from '@jsverse/transloco';
import { PlayerSlotListComponent } from 'tikal-ui-components';

@Component({
  selector: 'tikal-active-lobby',
  imports: [TranslocoDirective, PlayerSlotListComponent],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobbyComponent {
  readonly activeLobbyStore = inject(ActiveLobbyStore);

  constructor() {
    this.activeLobbyStore.loadActiveLobby();
  }
}
