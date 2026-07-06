import { Component, inject, input } from '@angular/core';
import { Player } from '../../models/player';
import { AccountStore } from '../../../../core/stores/account-store/account-store';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-lobby-player-list',
  imports: [TranslocoDirective],
  templateUrl: './lobby-player-list.html',
  styleUrl: './lobby-player-list.scss',
})
export class LobbyPlayerList {
  readonly accountStore = inject(AccountStore);

  readonly maxPlayers = input.required<number>();

  readonly players = input.required<Player[]>();

  readonly isLoading = input<boolean>(false);

  isMe(player: Player): boolean {
    return player.userId === this.accountStore.account()?.userId;
  }
}
