import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideTriangleAlert } from '@lucide/angular';

@Component({
  selector: 'app-lobby-not-found-overlay',
  imports: [TranslocoDirective, LucideTriangleAlert, RouterLink],
  templateUrl: './lobby-not-found-overlay.html',
  styleUrl: './lobby-not-found-overlay.scss',
})
export class LobbyNotFoundOverlay {}
