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
import { delay, distinctUntilChanged, pipe, switchMap, tap } from 'rxjs';
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

const initialState: LobbySummaryState = {
  lobbies: [],
  status: 'initial',
  totalCount: 0,
  filter: {
    pageSize: 8,
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

  withComputed(({ totalCount, status, filter }) => ({
    hasPrevious: computed(() => filter.pageNumber() > 1),

    hasNext: computed(() => totalCount() - filter.pageNumber() * filter.pageSize() > 0),

    maxPage: computed(() => Math.ceil(totalCount() / filter.pageSize())),

    isLoading: computed(() => status() === 'loading'),
  })),

  withMethods((store) => ({
    updatePageNumber(pageNumber: number): void {
      patchState(store, (state) => ({ filter: { ...state.filter, pageNumber } }));
    },

    updateSearchText(searchText: string): void {
      patchState(store, (state) => ({ filter: { ...state.filter, searchText } }));
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
        distinctUntilChanged(),
        tap(() => patchState(store, { status: 'loading' })),
        delay(3000),
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
                error: () => patchState(store, { status: 'error' }),
              }),
            );
        }),
      ),
    ),
  })),
);
