import { Component, computed, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountStore } from './core/stores/account-store/account-store';
import { AuthStore } from './core/stores/auth-store/auth-store';
import { Navbar } from './core/components/navbar/navbar';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-root',
  imports: [RouterOutlet, Navbar],
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
