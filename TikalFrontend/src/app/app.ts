import { Component, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet, RouterLinkWithHref, RouterLinkActive } from '@angular/router';
import { AccountStore } from './core/stores/account-store/account-store';
import { AuthStore } from './core/stores/auth-store/auth-store';
import { SidebarModule } from 'primeng/sidebar';
import { ButtonModule } from 'primeng/button';
import { User } from '@primeicons/angular/user';
import { Home } from '@primeicons/angular/home';
import { Sidebar } from '@primeicons/angular/sidebar';
import { Crown } from '@primeicons/angular/crown';
import { ButtonComponent } from 'tikal-ui-components';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-root',
  imports: [
    RouterOutlet,
    SidebarModule,
    ButtonModule,
    ButtonComponent,
    Home,
    User,
    Crown,
    Sidebar,
    RouterLinkWithHref,
    RouterLinkActive,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly accountStore = inject(AccountStore);

  private readonly authStore = inject(AuthStore);

  readonly initializationFailed = computed(
    () => this.accountStore.initializationFailed() || this.authStore.initializationFailed(),
  );

  toggleDarkMode(): void {
    const element = document.querySelector('html');
    element?.classList.toggle('dark');
  }
}
