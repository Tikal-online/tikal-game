import { Component, inject, signal } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
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
    DialogModule,
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

  readonly activeLobbyStore = inject(ActiveLobbyStore);

  private readonly lobbyData = signal<LobbyData>({ name: '', maxPlayers: 4 });

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
            await this.router.navigate(['/Lobbies/me']);
          }
        },
      },
    },
  );
}
