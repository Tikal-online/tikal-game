import { Routes } from '@angular/router';
import { Background } from './core/components/background/background';
import { Home } from './core/components/home/home';

export const routes: Routes = [
  {
    path: '',
    component: Background,
    children: [
      {
        path: '',
        component: Home,
      },
    ],
  },
];
