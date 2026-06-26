import { patchState, signalStore, withMethods, withProps, withState } from '@ngrx/signals';
import { LobbyService, LobbySummary } from '../../services/lobby/lobby-service';
import { inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { debounceTime, distinctUntilChanged, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type LobbySummaryState = {
  lobbies: LobbySummary[];
  status: 'initial' | 'loading' | 'loaded' | 'error';
  filter: {
    pageSize: number;
    pageNumber: number;
    searchText: string;
  };
};

const initialState: LobbySummaryState = {
  lobbies: [],
  status: 'initial',
  filter: {
    pageSize: 10,
    pageNumber: 1,
    searchText: '',
  },
};

export const LobbySummaryStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
  })),

  withMethods((store) => ({
    updatePageSize(pageSize: number): void {
      patchState(store, (state) => ({ filter: { ...state.filter, pageSize } }));
    },

    updatePageNumber(pageNumber: number): void {
      patchState(store, (state) => ({ filter: { ...state.filter, pageNumber } }));
    },

    updateSearchText(searchText: string): void {
      patchState(store, (state) => ({ filter: { ...state.filter, searchText } }));
    },

    loadLobbies: rxMethod<{ pageSize: number; pageNumber: number; searchText: string }>(
      pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => patchState(store, { status: 'loading' })),
        switchMap((query) => {
          return store._lobbyService
            .getLobbiesSummary(query.pageSize, query.pageNumber, query.searchText)
            .pipe(
              tapResponse({
                next: (lobbies) => patchState(store, { lobbies, status: 'loaded' }),
                error: () => patchState(store, { status: 'error' }),
              }),
            );
        }),
      ),
    ),
  })),
);
