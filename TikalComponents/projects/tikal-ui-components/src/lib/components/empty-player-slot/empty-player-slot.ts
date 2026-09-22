import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'tikal-empty-player-slot',
  imports: [TranslocoDirective],
  templateUrl: './empty-player-slot.html',
  styleUrl: './empty-player-slot.scss',
})
export class EmptyPlayerSlotComponent {}
