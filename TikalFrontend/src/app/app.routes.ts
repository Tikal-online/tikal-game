import { Routes } from '@angular/router';
import { Home } from './core/components/home/home';
import { CreateAccount } from './core/components/create-account/create-account';
import { Lobbies } from './modules/lobbies/components/lobbies/lobbies';
import { isAuthenticated } from './core/route-guards/is-authenticated/is-authenticated-guard';
import { hasAccount } from './core/route-guards/has-account/has-account-guard';
import { hasNoAccount } from './core/route-guards/has-no-account/has-no-account-guard';
import { CreateLobby } from './modules/lobbies/components/create-lobby/create-lobby';
import { ActiveLobby } from './modules/lobbies/components/active-lobby/active-lobby';
import { PreviewLobby } from './modules/lobbies/components/preview-lobby/preview-lobby';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'lobbies',
    canActivate: [isAuthenticated, hasAccount],
    children: [
      { path: '', component: Lobbies },
      { path: 'me', component: ActiveLobby },
      { path: 'create', component: CreateLobby },
      { path: ':id', component: PreviewLobby },
    ],
  },
  {
    path: 'createAccount',
    component: CreateAccount,
    canActivate: [isAuthenticated, hasNoAccount],
  },
];
