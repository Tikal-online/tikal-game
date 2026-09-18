import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withProps,
  withState,
} from '@ngrx/signals';
import { LobbyService, LobbySummary } from '../../services/lobby/lobby-service';
import { computed, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { debounceTime, pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';

type LobbySummaryState = {
  lobbies: LobbySummary[];
  status: 'initial' | 'loading' | 'loaded' | 'error';
  totalCount: number;
  filter: {
    pageSize: number;
    pageNumber: number;
    searchText: string;
    // a helper to trigger a refresh without changing any filters
    refreshTrigger: boolean;
  };
};

const placeHolderLobby: LobbySummary = {
  name: '',
  maxPlayers: 0,
  currentPlayers: 0,
  id: '',
};

const initialState: LobbySummaryState = {
  lobbies: [],
  status: 'initial',
  totalCount: 0,
  filter: {
    pageSize: 15,
    pageNumber: 1,
    searchText: '',
    refreshTrigger: false,
  },
};

export const LobbySummaryStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),

  withProps(() => ({
    _lobbyService: inject(LobbyService),
  })),

  withComputed(({ lobbies, status, totalCount, filter }) => ({
    isLoading: computed(() => status() === 'loading'),

    noLobbiesFound: computed(() => status() === 'loaded' && lobbies().length === 0),

    hasMultiplePages: computed(() => totalCount() > filter.pageSize()),

    // while loading we want to provide placeholders to render skeletons
    displayLobbies: computed(() =>
      status() === 'loading'
        ? Array.from({ length: filter.pageSize() }, (_, i) => {
            return { ...placeHolderLobby, id: `${i}` };
          })
        : lobbies(),
    ),
  })),

  withMethods((store) => ({
    updatePageNumber(pageNumber: number): void {
      patchState(store, (state) => ({ filter: { ...state.filter, pageNumber } }));
    },

    updatePageSize(pageSize: number): void {
      patchState(store, (state) => ({ filter: { ...state.filter, pageSize } }));
    },

    updateSearchText(searchText: string): void {
      patchState(store, (state) => ({ filter: { ...state.filter, pageNumber: 1, searchText } }));
    },

    refresh(): void {
      patchState(store, (state) => ({
        filter: { ...state.filter, refreshTrigger: !state.filter.refreshTrigger },
      }));
    },

    loadLobbies: rxMethod<{
      pageSize: number;
      pageNumber: number;
      searchText: string;
      refreshTrigger: boolean;
    }>(
      pipe(
        tap(() => patchState(store, { status: 'loading' })),
        debounceTime(300),
        switchMap((query) => {
          return store._lobbyService
            .getLobbiesSummary(query.pageSize, query.pageNumber, query.searchText)
            .pipe(
              tapResponse({
                next: (paginatedResult) =>
                  patchState(store, {
                    lobbies: paginatedResult.data,
                    totalCount: paginatedResult.totalCount,
                    status: 'loaded',
                  }),
                error: () => patchState(store, { lobbies: [], totalCount: 0, status: 'error' }),
              }),
            );
        }),
      ),
    ),
  })),
);
