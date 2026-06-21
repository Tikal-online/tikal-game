import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideShovel } from '@lucide/angular';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [TranslocoDirective, LucideShovel, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {}
