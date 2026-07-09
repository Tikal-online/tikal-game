import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideTriangleAlert } from '@lucide/angular';

@Component({
  selector: 'app-lobby-full-overlay',
  imports: [TranslocoDirective, LucideTriangleAlert, RouterLink],
  templateUrl: './lobby-full-overlay.html',
  styleUrl: './lobby-full-overlay.scss',
})
export class LobbyFullOverlay {}
