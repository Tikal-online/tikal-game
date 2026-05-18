import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountStore } from './core/stores/account-store/account-store';
import { AuthStore } from './core/stores/auth-store/auth-store';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly accountStore = inject(AccountStore);

  private readonly authStore = inject(AuthStore);

  readonly initializing = computed(
    () => this.accountStore.loadingStatus() === 'loading' || this.authStore.status() === 'loading',
  );

  readonly initializationFailed = computed(
    () =>
      this.accountStore.loadingStatus() === 'serverError' ||
      this.authStore.status() === 'serverError',
  );
}
