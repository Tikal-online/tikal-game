import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AccountStore } from './core/stores/account-store/account-store';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  readonly accountStore = inject(AccountStore);

  readonly initializing = computed(() => this.accountStore.loadingStatus() === 'loading');

  readonly initializationFailed = computed(
    () => this.accountStore.loadingStatus() === 'serverError',
  );
}
