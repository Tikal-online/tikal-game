import { Routes } from '@angular/router';
import { ActiveGameComponent } from './components/active-game/active-game';
import { ActiveGameService } from './services/active-game/active-game-service';
import { ActiveGameStore } from './stores/active-game/active-game-store';

const routes: Routes = [
  {
    path: '',
    providers: [ActiveGameService, ActiveGameStore],
    children: [{ path: 'me', component: ActiveGameComponent }],
  },
];

export default routes;
