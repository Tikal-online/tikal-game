import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PreviewLobbyStore } from '../../stores/lobby/preview-lobby-store';
import { PlayerSlotListComponent, ButtonComponent } from 'tikal-ui-components';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'tikal-lobby-preview',
  imports: [PlayerSlotListComponent, TranslocoDirective, ButtonComponent, RouterLink],
  templateUrl: './lobby-preview.html',
  styleUrl: './lobby-preview.scss',
})
export class LobbyPreviewComponent {
  private readonly router = inject(Router);

  private readonly route = inject(ActivatedRoute);

  private readonly previewLobbyStore = inject(PreviewLobbyStore);

  readonly isJoining = signal(false);

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.previewLobbyStore.loadLobby(id);
  }

  async join(): Promise<void> {
    this.isJoining.set(true);

    const result = await this.previewLobbyStore
      .joinLobby()
      .finally(() => this.isJoining.set(false));

    if (result.isOk()) {
      await this.router.navigate(['/Lobbies/me']);
    }
  }
}
