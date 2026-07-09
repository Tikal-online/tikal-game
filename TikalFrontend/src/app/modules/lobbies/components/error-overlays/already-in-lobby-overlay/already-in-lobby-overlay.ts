import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideTriangleAlert } from '@lucide/angular';

@Component({
  selector: 'app-already-in-lobby-overlay',
  imports: [TranslocoDirective, LucideTriangleAlert, RouterLink],
  templateUrl: './already-in-lobby-overlay.html',
  styleUrl: './already-in-lobby-overlay.scss',
})
export class AlreadyInLobbyOverlay {}
