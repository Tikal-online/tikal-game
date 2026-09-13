import { Component, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountStore } from './core/stores/account-store/account-store';
import { AuthStore } from './core/stores/auth-store/auth-store';
import { SidebarModule } from 'primeng/sidebar';
import { ButtonModule } from 'primeng/button';
import { User } from '@primeicons/angular/user';
import { Home } from '@primeicons/angular/home';
import { Sidebar } from '@primeicons/angular/sidebar';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-root',
  imports: [RouterOutlet, SidebarModule, ButtonModule, Home, User, Sidebar],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly accountStore = inject(AccountStore);

  private readonly authStore = inject(AuthStore);

  readonly initializationFailed = computed(
    () => this.accountStore.initializationFailed() || this.authStore.initializationFailed(),
  );
}
