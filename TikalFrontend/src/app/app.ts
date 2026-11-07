import { Component, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent, HeaderLink } from 'tikal-ui-components';
import { AccountStore } from './core/stores/account-store/account-store';
import { AuthStore } from './core/stores/auth-store/auth-store';
import { ThemeStore } from './core/stores/theme-store/theme-store';
import { ActiveLobbyStore } from './modules/lobbies/stores/active-lobby/active-lobby-store';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
})
export class App {
  private readonly accountStore = inject(AccountStore);

  private readonly authStore = inject(AuthStore);

  private readonly themeStore = inject(ThemeStore);

  private readonly activeLobbyStore = inject(ActiveLobbyStore);

  readonly links: readonly HeaderLink[] = [
    { label: 'Lobbies', route: '/Lobbies' },
    { label: 'Profile', route: '/Profile' },
  ];

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
