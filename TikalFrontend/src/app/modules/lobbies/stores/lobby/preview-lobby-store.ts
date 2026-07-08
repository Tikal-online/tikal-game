import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import { Lobby } from '../../models/lobby';
import { inject } from '@angular/core';
import { LobbyService } from '../../services/lobby/lobby-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type PreviewLobbyState = {
  lobby: Lobby | null;
  status: 'initial' | 'loading' | 'loaded' | 'error';
};

const initialState: PreviewLobbyState = {
  lobby: null,
  status: 'initial',
};

export const PreviewLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
  })),

  withMethods((store) => ({
    loadLobby: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { status: 'loading' })),
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
  })),
);
