import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideSearch } from '@lucide/angular';

@Component({
  selector: 'app-lobbies-list-not-found',
  imports: [LucideSearch, TranslocoDirective],
  templateUrl: './lobbies-list-not-found.html',
  styleUrl: './lobbies-list-not-found.scss',
})
export class LobbiesListNotFound {}
