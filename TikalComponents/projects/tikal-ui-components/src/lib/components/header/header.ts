import { Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonComponent } from '../button/button';
import { NavLinkComponent } from '../nav-link/nav-link';

export type HeaderLink = {
  label: string;
  route: string | readonly unknown[];
  exact?: boolean;
};

@Component({
  selector: 'tikal-header',
  imports: [RouterLink, NavLinkComponent, ButtonComponent],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class HeaderComponent {
  /** What route should the brand link navigate to? */
  readonly homeRoute = input<string | readonly unknown[]>('/');

  /** What links should the main navigation display? */
  readonly links = input<readonly HeaderLink[]>([]);

  /** Is dark mode currently enabled? */
  readonly darkMode = input<boolean>(false);

  /** @ignore */
  readonly darkModeToggled = output<void>();
}
