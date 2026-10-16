import { Component, inject, signal } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';
import { ButtonComponent, InputComponent, MaxPlayerSelectComponent } from 'tikal-ui-components';
import { disabled, form, FormField, FormRoot, maxLength, required } from '@angular/forms/signals';

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
  readonly lobbySummaryStore = inject(LobbySummaryStore);

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
