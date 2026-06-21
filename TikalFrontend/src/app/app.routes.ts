import { Routes } from '@angular/router';
import { Home } from './core/components/home/home';
import { CreateAccount } from './core/components/create-account/create-account';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'createAccount',
    component: CreateAccount,
  },
];
