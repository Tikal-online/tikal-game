import { Component, inject, signal } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
import { ButtonComponent, InputComponent, MaxPlayerSelectComponent } from 'tikal-ui-components';
import { disabled, form, FormField, FormRoot, maxLength, required } from '@angular/forms/signals';
import { CreateLobbyStore } from '../../stores/active-lobby/create-lobby-store';

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
  ],
  templateUrl: './create-lobby.html',
  styleUrl: './create-lobby.scss',
})
export class CreateLobbyDialog {
  readonly createLobbyStore = inject(CreateLobbyStore);

  private readonly lobbyData = signal<LobbyData>({ name: '', maxPlayers: 4 });

  readonly lobbyForm = form(
    this.lobbyData,
    (schemaPath) => {
      required(schemaPath.name);
      maxLength(schemaPath.name, 30);
      disabled(schemaPath, { when: () => this.lobbyForm().submitting() });
    },
    {
      submission: {
        action: async (field) => {
          const name = field().value().name;

          return;
        },
      },
    },
  );
}
