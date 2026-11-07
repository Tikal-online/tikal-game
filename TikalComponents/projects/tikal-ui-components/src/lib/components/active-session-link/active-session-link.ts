import { Component, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TranslocoDirective } from '@jsverse/transloco';

export type SessionType = 'lobby' | 'game';

@Component({
  selector: 'tikal-active-session-link',
  imports: [RouterLink, RouterLinkActive, ButtonModule, TranslocoDirective],
  templateUrl: './active-session-link.html',
  styleUrl: './active-session-link.scss',
})
export class ActiveSessionLinkComponent {
  /** Is the player in a lobby or in a game? */
  readonly type = input.required<SessionType>();

  /** What is the name of the lobby or game? */
  readonly name = input.required<string>();

  /** What route should the link navigate to? */
  readonly route = input.required<string | readonly unknown[]>();
}
