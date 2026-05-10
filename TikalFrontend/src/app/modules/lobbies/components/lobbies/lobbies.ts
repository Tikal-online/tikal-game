import { Component, inject } from '@angular/core';
import { AuthStore } from '../../../../core/stores/auth-store/auth-store';

@Component({
  selector: 'app-lobbies-page',
  imports: [],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class LobbiesPage {
  private readonly authStore = inject(AuthStore);

  logoutUrl = this.authStore.logoutUrl;
}
