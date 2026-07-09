import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { Lobby } from '../../models/lobby';
import { computed, inject } from '@angular/core';
import { LobbyService } from '../../services/lobby/lobby-service';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { Router } from '@angular/router';

type ActiveLobbyState = {
  lobby: Lobby | null;
  status: 'initial' | 'loading' | 'loaded' | 'error';
  leavingStatus: 'initial' | 'leaving' | 'error';
};

const initialState: ActiveLobbyState = {
  lobby: null,
  status: 'initial',
  leavingStatus: 'initial',
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
    _router: inject(Router),
  })),

  withComputed(({ status }) => ({
    isLoading: computed(() => status() === 'loading'),
  })),

  withMethods((store) => ({
    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { status: 'loading', leavingStatus: 'initial' })),
        switchMap(() => {
          return store._lobbyService.getActiveLobby().pipe(
            tapResponse({
              next: (result) => patchState(store, { lobby: result, status: 'loaded' }),
              error: () => patchState(store, { status: 'error' }),
            }),
          );
        }),
      ),
    ),

    leaveLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { leavingStatus: 'leaving' })),
        switchMap(() => {
          return store._lobbyService.leaveLobby().pipe(
            tapResponse({
              next: () => store._router.navigate(['/lobbies']),
              error: () => patchState(store, { leavingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
