import { Component, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent, HeaderLink } from 'tikal-ui-components';
import { AccountStore } from './core/stores/account-store/account-store';
import { AuthStore } from './core/stores/auth-store/auth-store';
import { ThemeStore } from './core/stores/theme-store/theme-store';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly accountStore = inject(AccountStore);

  private readonly authStore = inject(AuthStore);

  private readonly themeStore = inject(ThemeStore);

  readonly links: readonly HeaderLink[] = [
    { label: 'Lobbies', route: '/Lobbies' },
    { label: 'Profile', route: '/Profile' },
  ];

  readonly initializationFailed = computed(
    () => this.accountStore.initializationFailed() || this.authStore.initializationFailed(),
  );

  toggleDarkMode(): void {
    this.themeStore.toggleDarkMode();
  }

  usesDarkMode(): boolean {
    return this.themeStore.isDarkMode();
  }
}
