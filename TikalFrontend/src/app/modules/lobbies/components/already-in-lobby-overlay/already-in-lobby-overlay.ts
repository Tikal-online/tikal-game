import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-already-in-lobby-overlay',
  imports: [TranslocoDirective],
  templateUrl: './already-in-lobby-overlay.html',
  styleUrl: './already-in-lobby-overlay.scss',
})
export class AlreadyInLobbyOverlay {}
