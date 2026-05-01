import { Component, computed, inject, Signal } from '@angular/core';
import { AuthService, Session } from '../Services/auth-service';

@Component({
  selector: 'app-user-session',
  imports: [],
  templateUrl: './user-session.html',
  styleUrl: './user-session.scss',
})
export class UserSessionComponent {
  private readonly auth = inject(AuthService);
  readonly session: Signal<Session> = this.auth.session;
  isAuthenticated = this.auth.isAuthenticated;
  isAnonymous = this.auth.isAnonymous;

  // Computed signal for claims
  readonly claims = computed(() => this.session() || []);
}
