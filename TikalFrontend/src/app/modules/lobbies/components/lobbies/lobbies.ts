import { Component, ChangeDetectionStrategy } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-lobbies-page',
  imports: [TranslocoDirective],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class LobbiesPage {}
