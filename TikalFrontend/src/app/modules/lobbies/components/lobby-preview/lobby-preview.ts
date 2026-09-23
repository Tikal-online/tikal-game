import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PreviewLobbyStore } from '../../stores/lobby/preview-lobby-store';
import { PlayerSlotListComponent } from 'tikal-ui-components';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'tikal-lobby-preview',
  imports: [PlayerSlotListComponent, TranslocoDirective],
  templateUrl: './lobby-preview.html',
  styleUrl: './lobby-preview.scss',
})
export class LobbyPreviewComponent {
  private readonly route = inject(ActivatedRoute);

  readonly previewLobbyStore = inject(PreviewLobbyStore);

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.previewLobbyStore.loadLobby(id);
  }
}
