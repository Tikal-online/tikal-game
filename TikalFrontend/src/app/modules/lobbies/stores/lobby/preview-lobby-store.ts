import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import { Lobby } from '../../models/lobby';
import { inject } from '@angular/core';
import { LobbyService } from '../../services/lobby/lobby-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { Router } from '@angular/router';

type PreviewLobbyState = {
  lobby: Lobby | null;
  status: 'initial' | 'loading' | 'loaded' | 'error';
  joiningStatus: 'initial' | 'joining' | 'notFound' | 'full' | 'alreadyInLobby' | 'error';
};

const initialState: PreviewLobbyState = {
  lobby: null,
  status: 'initial',
  joiningStatus: 'initial',
};

export const PreviewLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
    _router: inject(Router),
  })),

  withMethods((store) => ({
    loadLobby: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { status: 'loading', joiningStatus: 'initial' })),
        switchMap((id) => {
          return store._lobbyService.getLobby(id).pipe(
            tapResponse({
              next: (result) => patchState(store, { lobby: result, status: 'loaded' }),
              error: () => patchState(store, { status: 'error' }),
            }),
          );
        }),
      ),
    ),

    joinLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { joiningStatus: 'joining' })),
        switchMap(() => {
          return store._lobbyService.joinLobby(store.lobby()!.id).pipe(
            tapResponse({
              next: (result) => {
                if (result.isErr()) {
                  switch (result.error.type) {
                    case 'PlayerAlreadyInALobby':
                      patchState(store, { joiningStatus: 'alreadyInLobby' });
                      break;
                    case 'LobbyNotFound':
                      patchState(store, { joiningStatus: 'notFound' });
                      break;
                    case 'LobbyFull':
                      patchState(store, { joiningStatus: 'full' });
                      break;
                  }
                } else {
                  store._router.navigate(['/lobbies/me']);
                }
              },
              error: () => patchState(store, { joiningStatus: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
