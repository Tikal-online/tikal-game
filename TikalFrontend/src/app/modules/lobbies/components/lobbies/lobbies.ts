import { Component, inject } from '@angular/core';
import { ButtonComponent, InputComponent } from 'tikal-ui-components';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';

@Component({
  selector: 'tikal-lobbies',
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
  imports: [ButtonComponent, InputComponent],
})
export class LobbiesComponent {
  readonly lobbiesSummaryStore = inject(LobbySummaryStore);

  constructor() {
    const filter = this.lobbiesSummaryStore.filter;

    this.lobbiesSummaryStore.loadLobbies(filter);
  }
}
