import { Component, inject } from '@angular/core';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';
import { InputComponent, ButtonComponent } from 'tikal-ui-components';

@Component({
  selector: 'tikal-lobbies-header',
  imports: [InputComponent, ButtonComponent],
  templateUrl: './lobbies-header.html',
  styleUrl: './lobbies-header.scss',
})
export class LobbiesHeaderComponent {
  readonly lobbiesSummaryStore = inject(LobbySummaryStore);
}
