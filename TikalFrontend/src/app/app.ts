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
import { Trophy } from '@primeicons/angular/trophy';
import { ButtonComponent } from 'tikal-ui-components';
import { ThemeStore } from './core/stores/theme-store/theme-store';
import { ActiveLobbyStore } from './modules/lobbies/stores/active-lobby/active-lobby-store';

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
    Trophy,
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

  private readonly themeStore = inject(ThemeStore);

  readonly activeLobbyStore = inject(ActiveLobbyStore);

  readonly initializationFailed = computed(
    () => this.accountStore.initializationFailed() || this.authStore.initializationFailed(),
  );

  constructor() {
    this.activeLobbyStore.loadActiveLobby();
  }

  toggleDarkMode(): void {
    this.themeStore.toggleDarkMode();
  }

  usesDarkMode(): boolean {
    return this.themeStore.isDarkMode();
  }
}
