import {
  afterNextRender,
  Component,
  DestroyRef,
  effect,
  ElementRef,
  inject,
  input,
  signal,
  viewChild,
} from '@angular/core';
import { BoardView } from '../../board/board-view';
import { Tile } from '../../models/tile';

@Component({
  selector: 'tikal-game-board',
  templateUrl: './game-board.html',
  styleUrl: './game-board.scss',
})
export class GameBoardComponent {
  readonly tiles = input.required<readonly Tile[]>();

  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  private readonly canvas = viewChild.required<ElementRef<HTMLCanvasElement>>('canvas');

  private readonly view = signal<BoardView | null>(null);

  constructor() {
    let resizeObserver: ResizeObserver | null = null;

    // WebGL needs the canvas in the DOM, so the view is only created after the first render
    afterNextRender(() => {
      const view = new BoardView(this.canvas().nativeElement);

      const element = this.host.nativeElement;
      resizeObserver = new ResizeObserver(() =>
        view.resize(element.clientWidth, element.clientHeight),
      );
      resizeObserver.observe(element);
      view.resize(element.clientWidth, element.clientHeight);

      this.view.set(view);
    });

    // runs once the view exists and again whenever the tiles change
    effect(() => this.view()?.setTiles(this.tiles()));

    inject(DestroyRef).onDestroy(() => {
      resizeObserver?.disconnect();
      this.view()?.dispose();
    });
  }
}
