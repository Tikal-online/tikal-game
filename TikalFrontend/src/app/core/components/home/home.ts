import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideShovel } from '@lucide/angular';

@Component({
  selector: 'app-home',
  imports: [TranslocoDirective, LucideShovel],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {}
