import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import {AuthService} from '../Services/auth-service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'nav-menu',
  imports: [RouterLink],
  templateUrl: './nav-menu.html',
  styleUrl: './nav-menu.scss'
})
export class NavMenuComponent {
  private auth = inject(AuthService);
  public authenticated = this.auth.isAuthenticated;
  public anonymous = this.auth.isAnonymous;
  public logoutUrl = this.auth.logoutUrl;
  public loginUrl = `${environment.backend_url}/bff/login?returnUrl=${window.location.origin}`
}