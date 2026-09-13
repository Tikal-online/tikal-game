import { Routes } from '@angular/router';
import { ProfileComponent } from './modules/profile/components/profile/profile';
import { LobbiesComponent } from './modules/lobbies/components/lobbies/lobbies';

export const routes: Routes = [
  { path: '', redirectTo: 'Lobbies', pathMatch: 'full' },
  { path: 'Profile', component: ProfileComponent },
  { path: 'Lobbies', component: LobbiesComponent },
];
