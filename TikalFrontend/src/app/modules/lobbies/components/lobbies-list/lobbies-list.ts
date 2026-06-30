import { Component, inject } from '@angular/core';
import { LucideGlobe, LucideUsers } from '@lucide/angular';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';
import { LobbiesListPagination } from './lobbies-list-pagination/lobbies-list-pagination';
import { LobbiesListHeader } from './lobbies-list-header/lobbies-list-header';
import { LobbiesListNotFound } from './lobbies-list-not-found/lobbies-list-not-found';
import { LobbiesListError } from './lobbies-list-error/lobbies-list.error';
import { TranslocoDirective } from '@jsverse/transloco';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-lobbies-list',
  imports: [
    LucideGlobe,
    LucideUsers,
    LobbiesListPagination,
    LobbiesListHeader,
    LobbiesListNotFound,
    LobbiesListError,
    TranslocoDirective,
    RouterLink,
  ],
  templateUrl: './lobbies-list.html',
  styleUrl: './lobbies-list.scss',
})
export class LobbiesList {
  readonly lobbySummaryStore = inject(LobbySummaryStore);

  constructor() {
    const filter = this.lobbySummaryStore.filter;

    this.lobbySummaryStore.loadLobbies(filter);
  }
}
