import { Component, inject, signal } from '@angular/core';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { ButtonComponent, InputComponent, MaxPlayerSelectComponent } from 'tikal-ui-components';
import { disabled, form, FormField, FormRoot, maxLength, required } from '@angular/forms/signals';
import { Router } from '@angular/router';
import { ActiveLobbyStore } from '../../stores/active-lobby/active-lobby-store';
import { TranslocoDirective } from '@jsverse/transloco';

type LobbyData = {
  name: string;
  maxPlayers: number;
};

@Component({
  selector: 'tikal-create-lobby-dialog',
  imports: [
    ButtonComponent,
    FormRoot,
    FormField,
    InputComponent,
    MaxPlayerSelectComponent,
    TranslocoDirective,
  ],
  templateUrl: './create-lobby.html',
  styleUrl: './create-lobby.scss',
})
export class CreateLobbyDialog {
  private readonly router = inject(Router);

  private readonly activeLobbyStore = inject(ActiveLobbyStore);

  private readonly lobbyData = signal<LobbyData>({ name: '', maxPlayers: 4 });

  readonly dialogRef = inject(DynamicDialogRef);

  readonly lobbyForm = form(
    this.lobbyData,
    (schemaPath) => {
      required(schemaPath.name);
      required(schemaPath.maxPlayers);
      maxLength(schemaPath.name, 30);
      disabled(schemaPath, { when: () => this.lobbyForm().submitting() });
    },
    {
      submission: {
        action: async (field) => {
          const result = await this.activeLobbyStore.createLobby(field().value());

          if (result.isOk()) {
            this.dialogRef.close();
            await this.router.navigate(['/Lobbies/me']);
          }
        },
      },
    },
  );
}
