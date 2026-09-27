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
import { filter, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { ActiveLobbyService } from '../../services/active-lobby/active-lobby-service';

type ActiveLobbyState = {
  lobby: Lobby | null;
  loadingStatus: 'initial' | 'loading' | 'loaded' | 'error';
  leavingStatus: 'initial' | 'leaving' | 'left' | 'error';
};

const initialStatus: ActiveLobbyState = {
  lobby: null,
  loadingStatus: 'initial',
  leavingStatus: 'initial',
};

export const ActiveLobbyStore = signalStore(
  { providedIn: 'root' },

  withState(initialStatus),

  withProps(() => ({
    _activeLobbyService: inject(ActiveLobbyService),
  })),

  withComputed(({ loadingStatus, leavingStatus, lobby }) => ({
    isLoading: computed(() => loadingStatus() === 'loading'),

    isLeaving: computed(() => leavingStatus() === 'leaving'),

    isInLobby: computed(() => lobby() !== null),
  })),

  withMethods((store) => ({
    loadActiveLobby: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loadingStatus: 'loading', leavingStatus: 'initial' })),
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

    leaveActiveLobby: rxMethod<void>(
      pipe(
        filter(() => !!store.lobby()),
        tap(() => patchState(store, { leavingStatus: 'leaving' })),
        switchMap(() => {
          return store._activeLobbyService.leaveLobby(store.lobby()!.id).pipe(
            tapResponse({
              next: () =>
                patchState(store, { lobby: null, leavingStatus: 'left', loadingStatus: 'initial' }),
              error: () => patchState(store, { leavingStatus: 'error' }),
            }),
          );
        }),
      ),
    ),
  })),
);
