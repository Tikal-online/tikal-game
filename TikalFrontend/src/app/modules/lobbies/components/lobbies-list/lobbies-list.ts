import { Component, inject } from '@angular/core';
import { LucideChevronLeft, LucideChevronRight, LucideGlobe, LucideUsers } from '@lucide/angular';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';

@Component({
  selector: 'app-lobbies-list',
  imports: [LucideGlobe, LucideUsers, LucideChevronLeft, LucideChevronRight],
  templateUrl: './lobbies-list.html',
  styleUrl: './lobbies-list.scss',
})
export class LobbiesList {
  readonly lobbySummaryStore = inject(LobbySummaryStore);

  constructor() {
    const filter = this.lobbySummaryStore.filter;

    this.lobbySummaryStore.loadLobbies(filter);
  }

  incrementPageNumber(): void {
    this.lobbySummaryStore.updatePageNumber(this.lobbySummaryStore.filter.pageNumber() + 1);
  }

  decrementPageNumber(): void {
    this.lobbySummaryStore.updatePageNumber(this.lobbySummaryStore.filter.pageNumber() - 1);
  }
}
