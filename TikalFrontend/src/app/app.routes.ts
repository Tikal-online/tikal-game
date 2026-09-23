import { Routes } from '@angular/router';
import { ProfileComponent } from './modules/profile/components/profile/profile';
import { LobbiesComponent } from './modules/lobbies/components/lobbies/lobbies';
import { isAuthenticated } from './core/route-guards/is-authenticated/is-authenticated-guard';
import { hasAccount } from './core/route-guards/has-account/has-account-guard';
import { CreateAccountComponent } from './core/components/create-account/create-account';
import { hasNoAccount } from './core/route-guards/has-no-account/has-no-account-guard';
import { LobbyPreviewComponent } from './modules/lobbies/components/lobby-preview/lobby-preview';

export const routes: Routes = [
  { path: '', redirectTo: 'Lobbies', pathMatch: 'full' },
  { path: 'Profile', component: ProfileComponent, canActivate: [isAuthenticated, hasAccount] },
  {
    path: 'Lobbies',
    canActivate: [isAuthenticated, hasAccount],
    children: [
      { path: '', component: LobbiesComponent },
      { path: ':id', component: LobbyPreviewComponent },
    ],
  },
  {
    path: 'CreateAccount',
    component: CreateAccountComponent,
    canActivate: [isAuthenticated, hasNoAccount],
  },
];
