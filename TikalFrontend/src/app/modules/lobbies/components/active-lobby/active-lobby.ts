import { Component, DestroyRef, effect, inject, signal } from '@angular/core';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';
import { TranslocoDirective } from '@jsverse/transloco';
import {
  PlayerSlotListComponent,
  ButtonComponent,
  CountdownOverlayComponent,
} from 'tikal-ui-components';
import { Router } from '@angular/router';

@Component({
  selector: 'tikal-active-lobby',
  imports: [
    TranslocoDirective,
    PlayerSlotListComponent,
    ButtonComponent,
    CountdownOverlayComponent,
  ],
  templateUrl: './active-lobby.html',
  styleUrl: './active-lobby.scss',
})
export class ActiveLobbyComponent {
  private readonly router = inject(Router);

  readonly activeLobbyStore = inject(ActiveLobbyStore);

  readonly isLeaving = signal(false);

  constructor() {
    this.activeLobbyStore.loadActiveLobby();
    this.activeLobbyStore.connect();

    inject(DestroyRef).onDestroy(() => this.activeLobbyStore.disconnect());

    effect(() => {
      if (this.activeLobbyStore.isStarted()) {
        this.router.navigate(['/Games/me']);
      }
    });

    // still covers having no active lobby on page load and being removed via SignalR
    effect(() => {
      if (!this.activeLobbyStore.lobby() && !this.activeLobbyStore.isLoading()) {
        this.router.navigate(['/Lobbies']);
      }
    });
  }

  async leave(): Promise<void> {
    this.isLeaving.set(true);

    const result = await this.activeLobbyStore
      .leaveActiveLobby()
      .finally(() => this.isLeaving.set(false));

    if (result.isOk()) {
      await this.router.navigate(['/Lobbies']);
    }
  }
}
