import { Component, inject } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { Inbox } from '@primeicons/angular/inbox';
import { ButtonComponent } from 'tikal-ui-components';
import { CreateLobbyStore } from '../../stores/active-lobby/create-lobby-store';

@Component({
  selector: 'tikal-lobbies-not-found',
  imports: [Inbox, ButtonComponent, TranslocoDirective],
  templateUrl: './lobbies-not-found.html',
  styleUrl: './lobbies-not-found.scss',
})
export class LobbiesNotFoundComponent {
  readonly createLobbyStore = inject(CreateLobbyStore);
}
