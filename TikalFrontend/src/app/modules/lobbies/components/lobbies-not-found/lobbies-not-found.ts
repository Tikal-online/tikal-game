import { Component, inject } from '@angular/core';
import { TranslocoDirective, TranslocoService } from '@jsverse/transloco';
import { Inbox } from '@primeicons/angular/inbox';
import { ButtonComponent } from 'tikal-ui-components';
import { DialogService } from 'primeng/dynamicdialog';
import { CreateLobbyDialog } from '../create-lobby/create-lobby';

@Component({
  selector: 'tikal-lobbies-not-found',
  imports: [Inbox, ButtonComponent, TranslocoDirective],
  templateUrl: './lobbies-not-found.html',
  styleUrl: './lobbies-not-found.scss',
})
export class LobbiesNotFoundComponent {
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
