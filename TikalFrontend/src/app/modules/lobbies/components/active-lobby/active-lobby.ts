import { Component, effect, inject } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';
import { TranslocoDirective } from '@jsverse/transloco';
import { PlayerSlotListComponent, ButtonComponent } from 'tikal-ui-components';
import { Router } from '@angular/router';

@Component({
  selector: 'tikal-active-lobby',
  imports: [TranslocoDirective, PlayerSlotListComponent, ButtonComponent],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobbyComponent {
  private readonly router = inject(Router);

  readonly activeLobbyStore = inject(ActiveLobbyStore);

  constructor() {
    this.activeLobbyStore.loadActiveLobby();

    effect(() => {
      if (this.activeLobbyStore.leavingStatus() === 'left') {
        this.router.navigate(['/Lobbies']);
      }
    });
  }
}
