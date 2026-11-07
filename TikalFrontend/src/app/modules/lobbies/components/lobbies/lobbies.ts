import { Component, inject } from '@angular/core';
import { SkeletonComponent, PlayerCountComponent, ChipComponent } from 'tikal-ui-components';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';
import { TableLazyLoadEvent, TableModule } from 'primeng/table';
import { LobbiesHeaderComponent } from '../lobbies-header/lobbies-header';
import { TranslocoDirective } from '@jsverse/transloco';
import { LobbiesNotFoundComponent } from '../lobbies-not-found/lobbies-not-found';
import { RouterLink } from '@angular/router';

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
    RouterLink,
  ],
})
export class LobbiesComponent {
  readonly lobbiesSummaryStore = inject(LobbySummaryStore);

  readonly min = Math.min;

  // tokens scoped to this table: muted header row on a light surface, roomier cells and a separated paginator
  readonly tableTokens = {
    headerCell: {
      background: 'light-dark({surface.50}, {surface.950})',
      color: '{text.muted.color}',
      borderColor: '{content.border.color}',
      padding: '0.75rem 1rem',
    },
    bodyCell: {
      borderColor: 'light-dark({surface.200}, {surface.800})',
      padding: '0 1rem',
    },
    paginatorBottom: {
      borderColor: '{content.border.color}',
      borderWidth: '1px 0 0 0',
    },
  };

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
