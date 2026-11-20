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

  /** The drawn tile that still has to be placed, shown on the empty place under the mouse. */
  readonly pendingTile = input<Tile | null>(null);

  private readonly canvas = viewChild.required<ElementRef<HTMLCanvasElement>>('canvas');

  private readonly view = signal<BoardView | null>(null);

  constructor() {
    // WebGL needs the canvas in the DOM, so the view is only created after the first render
    afterNextRender(() => this.view.set(new BoardView(this.canvas().nativeElement)));

    // runs once the view exists and again whenever the tiles change
    effect(() => this.view()?.setTiles(this.tiles()));

    effect(() => this.view()?.setPendingTile(this.pendingTile()));

    inject(DestroyRef).onDestroy(() => this.view()?.dispose());
  }
}
