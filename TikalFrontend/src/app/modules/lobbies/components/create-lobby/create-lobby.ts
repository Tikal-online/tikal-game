import { Component, inject, signal } from '@angular/core';
import { form, FormField, FormRoot, max, maxLength, min, required } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { LucideArrowLeft } from '@lucide/angular';
import { AccountStore } from '../../../../core/stores/account-store/account-store';
import { LobbyPlayerList } from '../lobby-player-list/lobby-player-list';
import { Player } from '../../models/player';
import { LobbyPlayerListHeader } from '../lobby-player-list-header/lobby-player-list-header';
import { TranslocoDirective } from '@jsverse/transloco';
import { MaxPlayersSelection } from './max-players-selection/max-players-selection';
import { LobbyService } from '../../services/lobby/lobby-service';
import { firstValueFrom } from 'rxjs';

type LobbyData = {
  name: string;
  maxPlayers: number;
};

@Component({
  selector: 'app-create-lobby',
  imports: [
    LucideArrowLeft,
    RouterLink,
    FormRoot,
    FormField,
    LobbyPlayerList,
    LobbyPlayerListHeader,
    TranslocoDirective,
    MaxPlayersSelection,
  ],
  templateUrl: './create-lobby.html',
  styleUrl: './create-lobby.scss',
})
export class CreateLobby {
  private readonly accountStore = inject(AccountStore);

  private readonly lobbyService = inject(LobbyService);

  private readonly router = inject(Router);

  readonly defaultPlayer = signal<Player>({
    userId: this.accountStore.account()!.userId,
    name: this.accountStore.account()!.name,
    isOwner: true,
    isReady: false,
  });

  readonly lobbyData = signal<LobbyData>({
    name: `${this.accountStore.account()?.name}s Lobby`,
    maxPlayers: 4,
  });

  readonly lobbyForm = form(
    this.lobbyData,
    (schemaPath) => {
      required(schemaPath.name);
      maxLength(schemaPath.name, 30);
      required(schemaPath.maxPlayers);
      min(schemaPath.maxPlayers, 2);
      max(schemaPath.maxPlayers, 4);
    },
    {
      submission: {
        action: async (field) => {
          const data = field().value();

          const result = await firstValueFrom(
            this.lobbyService.createLobby(data.name, data.maxPlayers),
          );

          if (result.isOk()) {
            this.router.navigate(['/lobbies']);
            return;
          }

          return {
            kind: 'serverError',
          };
        },
      },
    },
  );

  setMaxPlayers(maxPlayers: number): void {
    this.lobbyData.update((data) => ({ ...data, maxPlayers: maxPlayers }));
  }
}
