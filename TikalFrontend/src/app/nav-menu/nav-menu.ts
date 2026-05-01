import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../Services/auth-service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'nav-menu',
  imports: [RouterLink],
  templateUrl: './nav-menu.html',
  styleUrl: './nav-menu.scss',
})
export class NavMenuComponent {
  private auth = inject(AuthService);
  authenticated = this.auth.isAuthenticated;
  anonymous = this.auth.isAnonymous;
  logoutUrl = this.auth.logoutUrl;
  loginUrl = `${environment.backend_url}/bff/login?returnUrl=${window.location.origin}`;
}
