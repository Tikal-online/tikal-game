import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';

@Component({
  selector: 'app-home',
  imports: [TranslocoDirective],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {}
