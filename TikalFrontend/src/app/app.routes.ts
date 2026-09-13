import { Routes } from '@angular/router';
import { ProfileComponent } from './modules/profile/components/profile/profile';
import { LobbiesComponent } from './modules/lobbies/components/lobbies/lobbies';
import { isAuthenticated } from './core/route-guards/is-authenticated/is-authenticated-guard';
import { hasAccount } from './core/route-guards/has-account/has-account-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'Lobbies', pathMatch: 'full' },
  { path: 'Profile', component: ProfileComponent, canActivate: [isAuthenticated, hasAccount] },
  { path: 'Lobbies', component: LobbiesComponent, canActivate: [isAuthenticated, hasAccount] },
];
