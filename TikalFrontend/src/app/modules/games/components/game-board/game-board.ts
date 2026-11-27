import {
  afterNextRender,
  Component,
  DestroyRef,
  effect,
  ElementRef,
  inject,
  input,
  output,
  signal,
  viewChild,
} from '@angular/core';
import { BoardView } from '../../board/board-view';
import { HexCoordinate, Tile } from '../../models/tile';

@Component({
  selector: 'tikal-game-board',
  templateUrl: './game-board.html',
  styleUrl: './game-board.scss',
})
export class GameBoardComponent {
  readonly tiles = input.required<readonly Tile[]>();

  /** The drawn tile that still has to be placed, shown on the empty place under the mouse. */
  readonly pendingTile = input<Tile | null>(null);

  /** Emits 60 degree steps the pending tile should be turned by, positive is clockwise. */
  readonly rotatePendingTile = output<number>();

  /** Emits the place the user clicked to put the pending tile on. */
  readonly placePendingTile = output<HexCoordinate>();

  private readonly canvas = viewChild.required<ElementRef<HTMLCanvasElement>>('canvas');

  private readonly view = signal<BoardView | null>(null);

  constructor() {
    afterNextRender(() => {
      const view = new BoardView(this.canvas().nativeElement, {
        onPlacePendingTile: (coordinate): void => this.placePendingTile.emit(coordinate),
      });
      this.view.set(view);
    });

    effect(() => this.view()?.setTiles(this.tiles()));

    effect(() => this.view()?.setPendingTile(this.pendingTile()));

    inject(DestroyRef).onDestroy(() => this.view()?.dispose());
  }

  // T turns the pending tile clockwise, shift + T counter-clockwise
  onKeyDown(event: KeyboardEvent): void {
    if (event.code !== 'KeyT' || event.repeat || event.ctrlKey || event.metaKey || event.altKey) {
      return;
    }

    this.rotatePendingTile.emit(event.shiftKey ? -1 : 1);
  }
}
