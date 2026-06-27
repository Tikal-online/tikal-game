import { Component, inject } from '@angular/core';
import { LucideChevronLeft, LucideChevronRight } from '@lucide/angular';
import { LobbySummaryStore } from '../../../stores/lobby/lobby-summary-store';

@Component({
  selector: 'app-lobbies-list-pagination',
  imports: [LucideChevronLeft, LucideChevronRight],
  templateUrl: './lobbies-list-pagination.html',
  styleUrl: './lobbies-list-pagination.scss',
})
export class LobbiesListPagination {
  readonly lobbySummaryStore = inject(LobbySummaryStore);

  incrementPageNumber(): void {
    this.lobbySummaryStore.updatePageNumber(this.lobbySummaryStore.filter.pageNumber() + 1);
  }

  decrementPageNumber(): void {
    this.lobbySummaryStore.updatePageNumber(this.lobbySummaryStore.filter.pageNumber() - 1);
  }
}
