import { Component, inject } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';

@Component({
  selector: 'tikal-create-lobby-dialog',
  imports: [DialogModule],
  templateUrl: './create-lobby.html',
  styleUrl: './create-lobby.scss',
})
export class CreateLobbyDialog {
  readonly lobbySummaryStore = inject(LobbySummaryStore);
}
