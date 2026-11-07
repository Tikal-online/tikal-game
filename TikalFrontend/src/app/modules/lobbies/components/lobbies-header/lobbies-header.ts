import { Component, inject } from '@angular/core';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';
import { InputComponent, ButtonComponent } from 'tikal-ui-components';
import { TranslocoDirective, TranslocoService } from '@jsverse/transloco';
import { DialogService } from 'primeng/dynamicdialog';
import { CreateLobbyDialog } from '../create-lobby/create-lobby';

@Component({
  selector: 'tikal-lobbies-header',
  imports: [InputComponent, ButtonComponent, TranslocoDirective],
  templateUrl: './lobbies-header.html',
  styleUrl: './lobbies-header.scss',
  providers: [DialogService],
})
export class LobbiesHeaderComponent {
  readonly lobbiesSummaryStore = inject(LobbySummaryStore);

  private readonly dialogService = inject(DialogService);

  private readonly transloco = inject(TranslocoService);

  showCreateDialog(): void {
    this.dialogService.open(CreateLobbyDialog, {
      header: this.transloco.translate('lobbies.create.header'),
      width: '24rem',
      modal: true,
      closable: true,
      closeOnEscape: true,
    });
  }
}
