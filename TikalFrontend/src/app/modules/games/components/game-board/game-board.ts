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
import { TranslocoDirective } from '@jsverse/transloco';
import { HintComponent } from 'tikal-ui-components';
import { BoardView } from '../../board/board-view';
import { HexCoordinate, Tile } from '../../models/tile';

@Component({
  selector: 'tikal-game-board',
  imports: [TranslocoDirective, HintComponent],
  templateUrl: './game-board.html',
  styleUrl: './game-board.scss',
})
export class GameBoardComponent {
  readonly tiles = input.required<readonly Tile[]>();

  /** The drawn tile at its coordinate, shown while the mouse is over a free place. */
  readonly pendingTile = input<Tile | null>(null);

  /** Whether the pending tile may be placed where it is, otherwise it is shown red. */
  readonly canPlacePendingTile = input<boolean>(false);

  /** Emits the free place under the mouse whenever it changes. */
  readonly hoveredPlaceChange = output<HexCoordinate>();

  /** Emits the free place the user clicked. */
  readonly placeClick = output<HexCoordinate>();

  /** Emits 60 degree steps the pending tile should be turned by, positive is clockwise. */
  readonly rotatePendingTile = output<number>();

  /** The keyboard controls only work while the board has the focus. */
  readonly hasFocus = signal(false);

  private readonly canvas = viewChild.required<ElementRef<HTMLCanvasElement>>('canvas');

  private readonly view = signal<BoardView | null>(null);

  constructor() {
    afterNextRender(() => {
      const view = new BoardView(this.canvas().nativeElement, {
        onHoveredPlaceChanged: (coordinate): void => this.hoveredPlaceChange.emit(coordinate),
        onPlaceClicked: (coordinate): void => this.placeClick.emit(coordinate),
      });
      this.view.set(view);
    });

    effect(() => this.view()?.setTiles(this.tiles()));

    effect(() => this.view()?.setPendingTile(this.pendingTile(), !this.canPlacePendingTile()));

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
