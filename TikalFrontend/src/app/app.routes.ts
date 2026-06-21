import { Routes } from '@angular/router';
import { Home } from './core/components/home/home';
import { CreateAccount } from './core/components/create-account/create-account';
import { Lobbies } from './modules/lobbies/components/lobbies/lobbies';
import { isAuthenticated } from './core/route-guards/is-authenticated/is-authenticated-guard';
import { hasAccount } from './core/route-guards/has-account/has-account-guard';
import { hasNoAccount } from './core/route-guards/has-no-account/has-no-account-guard';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'lobbies',
    component: Lobbies,
    canActivate: [isAuthenticated, hasAccount],
  },
  {
    path: 'createAccount',
    component: CreateAccount,
    canActivate: [isAuthenticated, hasNoAccount],
  },
];
