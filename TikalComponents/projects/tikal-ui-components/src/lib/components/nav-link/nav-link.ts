import { Component, computed, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'tikal-nav-link',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './nav-link.html',
  styleUrl: './nav-link.scss',
})
export class NavLinkComponent {
  /** What text should the link display? */
  readonly label = input.required<string>();

  /** What route should the link navigate to? */
  readonly route = input.required<string | readonly unknown[]>();

  /** Should the link only be marked active when the route matches exactly (not for child routes)? */
  readonly exact = input<boolean>(false);

  /** @ignore */
  readonly activeOptions = computed(() => ({ exact: this.exact() }));
}
