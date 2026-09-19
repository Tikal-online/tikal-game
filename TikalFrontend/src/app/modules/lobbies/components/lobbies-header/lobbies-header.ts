import { Component, inject } from '@angular/core';
import { LobbySummaryStore } from '../../stores/lobby/lobby-summary-store';
import { InputComponent, ButtonComponent } from 'tikal-ui-components';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'tikal-lobbies-header',
  imports: [InputComponent, ButtonComponent, TranslocoDirective],
  templateUrl: './lobbies-header.html',
  styleUrl: './lobbies-header.scss',
})
export class LobbiesHeaderComponent {
  readonly lobbiesSummaryStore = inject(LobbySummaryStore);
}
