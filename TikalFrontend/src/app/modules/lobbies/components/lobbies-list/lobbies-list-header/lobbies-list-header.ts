import { Component, inject } from '@angular/core';
import { LucideRefreshCw } from '@lucide/angular';
import { LobbySummaryStore } from '../../../stores/lobby/lobby-summary-store';

@Component({
  selector: 'app-lobbies-list-header',
  imports: [LucideRefreshCw],
  templateUrl: './lobbies-list-header.html',
  styleUrl: './lobbies-list-header.scss',
})
export class LobbiesListHeader {
  readonly lobbySummaryStore = inject(LobbySummaryStore);
}
