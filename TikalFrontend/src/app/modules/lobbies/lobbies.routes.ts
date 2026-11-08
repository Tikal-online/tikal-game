import { Routes } from '@angular/router';
import { DialogService } from 'primeng/dynamicdialog';
import { LobbiesComponent } from './components/lobbies/lobbies';
import { ActiveLobbyComponent } from './components/active-lobby/active-lobby';
import { LobbyPreviewComponent } from './components/lobby-preview/lobby-preview';
import { LobbyService } from './services/lobby/lobby-service';
import { ActiveLobbyService } from './services/active-lobby/active-lobby-service';
import { ActivePlayerService } from './services/active-lobby/active-player-service';
import { ActiveLobbyConnection } from './services/active-lobby/active-lobby-connection';
import { LobbySummaryStore } from './stores/lobby/lobby-summary-store';
import { PreviewLobbyStore } from './stores/lobby/preview-lobby-store';
import { ActiveLobbyStore } from './stores/active-lobby/active-lobby-store';

// DialogService is provided here and not on the components, otherwise dialogs opened through it
// are created with the root injector and cannot see the providers of this route
const routes: Routes = [
  {
    path: '',
    providers: [
      LobbyService,
      ActiveLobbyService,
      ActivePlayerService,
      ActiveLobbyConnection,
      LobbySummaryStore,
      PreviewLobbyStore,
      ActiveLobbyStore,
      DialogService,
    ],
    children: [
      { path: '', component: LobbiesComponent },
      { path: 'me', component: ActiveLobbyComponent },
      { path: ':id', component: LobbyPreviewComponent },
    ],
  },
];

export default routes;
