import { Component, inject, signal } from '@angular/core';
import { form, FormField, FormRoot, max, maxLength, min, required } from '@angular/forms/signals';
import { RouterLink } from '@angular/router';
import { LucideArrowLeft } from '@lucide/angular';
import { AccountStore } from '../../../../core/stores/account-store/account-store';

type LobbyData = {
  name: string;
  maxPlayers: number;
};

@Component({
  selector: 'app-create-lobby',
  imports: [LucideArrowLeft, RouterLink, FormRoot, FormField],
  templateUrl: './create-lobby.html',
  styleUrl: './create-lobby.scss',
})
export class CreateLobby {
  private readonly accountStore = inject(AccountStore);

  readonly lobbyData = signal<LobbyData>({
    name: `${this.accountStore.account()?.name}s Lobby`,
    maxPlayers: 4,
  });

  readonly lobbyForm = form(this.lobbyData, (schemaPath) => {
    required(schemaPath.name);
    maxLength(schemaPath.name, 30);
    required(schemaPath.maxPlayers);
    min(schemaPath.maxPlayers, 2);
    max(schemaPath.maxPlayers, 4);
  });
}
