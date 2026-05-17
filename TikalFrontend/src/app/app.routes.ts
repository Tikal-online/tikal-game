import { Routes } from '@angular/router';
import { Background } from './core/components/background/background';
import { Home } from './core/components/home/home';
import { LobbiesPage } from './modules/lobbies/components/lobbies/lobbies';
import { CreateAccount } from './core/components/create-account/create-account';
import { hasAccount } from './core/route-guards/has-account/has-account-guard';
import { isAuthenticated } from './core/route-guards/is-authenticated/is-authenticated-guard';

export const routes: Routes = [
  {
    path: '',
    component: Background,
    children: [
      {
        path: '',
        component: Home,
      },
      {
        path: 'lobbies',
        component: LobbiesPage,
        canActivate: [hasAccount],
      },
      {
        path: 'createAccount',
        component: CreateAccount,
        canActivate: [isAuthenticated],
      },
    ],
  },
];
