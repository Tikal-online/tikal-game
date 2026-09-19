import { Component } from '@angular/core';
import { Inbox } from '@primeicons/angular/inbox';
import { ButtonComponent } from 'tikal-ui-components';

@Component({
  selector: 'tikal-lobbies-not-found',
  imports: [Inbox, ButtonComponent],
  templateUrl: './lobbies-not-found.html',
  styleUrl: './lobbies-not-found.scss',
})
export class LobbiesNotFoundComponent {}
