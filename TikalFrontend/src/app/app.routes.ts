import { Routes } from '@angular/router';
import { isAuthenticated } from './core/route-guards/is-authenticated/is-authenticated-guard';
import { hasAccount } from './core/route-guards/has-account/has-account-guard';
import { CreateAccountComponent } from './core/components/create-account/create-account';
import { hasNoAccount } from './core/route-guards/has-no-account/has-no-account-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'Lobbies', pathMatch: 'full' },
  {
    path: 'Profile',
    canActivate: [isAuthenticated, hasAccount],
    loadChildren: () => import('./modules/profile/profile.routes'),
  },
  {
    path: 'Lobbies',
    canActivate: [isAuthenticated, hasAccount],
    loadChildren: () => import('./modules/lobbies/lobbies.routes'),
  },
  {
    path: 'Games',
    canActivate: [isAuthenticated, hasAccount],
    loadChildren: () => import('./modules/games/games.routes'),
  },
  {
    path: 'CreateAccount',
    component: CreateAccountComponent,
    canActivate: [isAuthenticated, hasNoAccount],
  },
];
