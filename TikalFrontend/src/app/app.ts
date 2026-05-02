import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavMenuComponent } from './nav-menu/nav-menu';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavMenuComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  title = 'angular.ssr';
}
