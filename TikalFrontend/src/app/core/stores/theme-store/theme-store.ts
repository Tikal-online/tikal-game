import { effect } from '@angular/core';
import { patchState, signalStore, withHooks, withMethods, withState } from '@ngrx/signals';
import { fromEvent } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

export type ThemeState = {
  isDarkMode: boolean;
};

function usesDarkMode(): boolean {
  const theme = localStorage.getItem('theme');

  if (theme) {
    return theme === 'theme-dark';
  }

  return window.matchMedia('(prefers-color-scheme: dark)').matches;
}

export const ThemeStore = signalStore(
  { providedIn: 'root' },

  withState<ThemeState>(() => ({
    isDarkMode: usesDarkMode(),
  })),

  withMethods((store) => ({
    toggleDarkMode(): void {
      patchState(store, { isDarkMode: !store.isDarkMode() });
    },
  })),

  withHooks({
    onInit(store) {
      // whenever dark mode is toggled update the root class and local storage
      effect(() => {
        const isDark = store.isDarkMode();
        document.documentElement.classList.toggle('dark', isDark);
        localStorage.setItem('theme', isDark ? 'theme-dark' : 'theme-light');
      });

      // update theme whenever system preferences change
      const media = window.matchMedia('(prefers-color-scheme: dark)');

      fromEvent<MediaQueryListEvent>(media, 'change')
        .pipe(takeUntilDestroyed())
        .subscribe(() => {
          patchState(store, {
            isDarkMode: window.matchMedia('(prefers-color-scheme: dark)').matches,
          });
        });
    },
  }),
);
