import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideTriangleAlert } from '@lucide/angular';

@Component({
  selector: 'app-already-in-lobby-overlay',
  imports: [TranslocoDirective, LucideTriangleAlert],
  templateUrl: './already-in-lobby-overlay.html',
  styleUrl: './already-in-lobby-overlay.scss',
})
export class AlreadyInLobbyOverlay {}
