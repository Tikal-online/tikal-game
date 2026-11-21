import { Routes } from '@angular/router';
import { ActiveGameComponent } from './components/active-game/active-game';
import { ActiveGameService } from './services/active-game/active-game-service';
import { ActiveGameConnection } from './services/active-game/active-game-connection';
import { ActiveGameStore } from './stores/active-game/active-game-store';

const routes: Routes = [
  {
    path: '',
    providers: [ActiveGameService, ActiveGameConnection, ActiveGameStore],
    children: [{ path: 'me', component: ActiveGameComponent }],
  },
];

export default routes;
