import { Component, inject } from '@angular/core';
import { SkeletonComponent, PlayerCountComponent, ChipComponent } from 'tikal-ui-components';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { LobbiesHeaderComponent } from '../lobbies-header/lobbies-header';
import { TranslocoDirective } from '@jsverse/transloco';
import { LobbiesNotFoundComponent } from '../lobbies-not-found/lobbies-not-found';

@Component({
  selector: 'tikal-lobbies',
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
  imports: [
    TableModule,
    SkeletonComponent,
    PlayerCountComponent,
    ChipComponent,
    LobbiesHeaderComponent,
    TranslocoDirective,
    LobbiesNotFoundComponent,
  ],
})
export class LobbiesComponent {
  readonly lobbiesSummaryStore = inject(LobbySummaryStore);

  constructor() {
    const filter = this.lobbiesSummaryStore.filter;

    this.lobbiesSummaryStore.loadLobbies(filter);
  }

  onPageChanged(event: TableLazyLoadEvent): void {
    const first = event.first ?? 0;
    const rows = event.rows ?? this.lobbiesSummaryStore.filter.pageSize();

    const pageIndex = Math.floor(first / rows);
    const pageNumber = pageIndex + 1;

    this.lobbiesSummaryStore.updatePageNumber(pageNumber);
    this.lobbiesSummaryStore.updatePageSize(rows);
  }
}
