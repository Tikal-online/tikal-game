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
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { ActiveLobbyService } from '../../services/active-lobby/active-lobby-service';

type ActiveLobbyState = {
  lobby: Lobby | null;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
};

const initialStatus: ActiveLobbyState = {
  lobby: null,
  loadingStatus: 'initial',
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialStatus),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
  })),

  withComputed(({ loadingStatus: status }) => ({
    isLoading: computed(() => status() === 'loading'),
  })),

  withMethods((store) => ({
    loadActiveLobby: rxMethod(
      pipe(
        tap(() => patchState(store, { loadingStatus: 'loading' })),
        switchMap(() => {
          return store._activeLobbyService.getActiveLobby().pipe(
            tapResponse({
              next: (result) => patchState(store, { lobby: result, loadingStatus: 'loaded' }),
              error: () => patchState(store, { lobby: null, loadingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
