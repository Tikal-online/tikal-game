import { DirectionalLight, Group, HemisphereLight, Scene, WebGLRenderer } from 'three';
import { HexCoordinate, Tile } from '../models/tile';
import { BoardCamera } from './utils/board-camera';
import { BoardClick } from './utils/board-click';
import { BoardHover, Hoverable } from './utils/board-hover';
import { boardCoordinates } from './board-layout';
import { BoardResources } from './board-resources';
import { coordinateKey, EDGE_OFFSETS } from './hex-layout';
import { SlotView } from './slot-view';
import { TileView } from './tile-view';

// how the board reports what the user does on it
export type BoardViewCallbacks = {
  /** A valid free place was clicked to put the pending tile on. */
  onPlacePendingTile: (coordinate: HexCoordinate) => void;
};

export class BoardView {
  private readonly scene = new Scene();

  private readonly renderer: WebGLRenderer;

  // Shared GPU resources
  private readonly resources = new BoardResources();

  // Utils
  private readonly boardCamera: BoardCamera;

  private readonly hover: BoardHover;

  private readonly click: BoardClick;

  // Placeholders
  private readonly slots = new Group();

  private readonly slotViews = new Map<string, SlotView>();

  // Tiles
  private readonly tiles = new Group();

  private readonly tileViews = new Map<string, TileView>();

  // Pending tile
  private pendingTile: Tile | null = null;

  private preview: TileView | null = null;

  // the slot the pending tile is shown on
  private previewSlot: SlotView | null | undefined = undefined;

  // the drawing buffer follows the canvas' css size
  private readonly resizeObserver = new ResizeObserver(() => this.resize());

  constructor(
    canvas: HTMLCanvasElement,
    private readonly callbacks: BoardViewCallbacks,
  ) {
    this.renderer = new WebGLRenderer({ canvas, antialias: true, alpha: true });
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

    this.boardCamera = new BoardCamera(canvas);
    this.hover = new BoardHover(canvas, this.boardCamera.camera);
    this.click = new BoardClick(
      canvas,
      this.boardCamera.camera,
      () => this.hoverables(),
      (target) => this.onBoardClick(target),
    );

    this.scene.add(this.slots, this.tiles);
    this.addSlots();
    this.addLights();

    this.resize();
    this.boardCamera.reset();
    this.resizeObserver.observe(canvas);

    this.startRenderLoop();
  }

  setTiles(tiles: readonly Tile[]): void {
    const keys = new Set<string>();

    for (const tile of tiles) {
      const key = coordinateKey(tile.coordinate);
      keys.add(key);

      const existing = this.tileViews.get(key);

      if (existing) {
        existing.update(tile);
      } else {
        const view = new TileView(tile, this.resources);
        this.tileViews.set(key, view);
        this.tiles.add(view.object);
      }
    }

    for (const [key, view] of this.tileViews) {
      if (!keys.has(key)) {
        this.tiles.remove(view.object);
        this.tileViews.delete(key);
      }
    }

    // a set tile replaces the slot it was set on
    for (const [key, slot] of this.slotViews) {
      slot.object.visible = !this.tileViews.has(key);
    }
  }

  setPendingTile(tile: Tile | null): void {
    this.pendingTile = tile;
    this.previewSlot = undefined;
  }

  dispose(): void {
    this.renderer.setAnimationLoop(null);
    this.resizeObserver.disconnect();

    this.boardCamera.dispose();
    this.hover.dispose();
    this.click.dispose();

    this.resources.dispose();
    this.renderer.dispose();
  }

  private resize(): void {
    const { clientWidth: width, clientHeight: height } = this.renderer.domElement;

    // a hidden canvas has no size, the camera would get an invalid aspect ratio
    if (width === 0 || height === 0) {
      return;
    }

    // false: the canvas size is controlled by css, only the drawing buffer is resized
    this.renderer.setSize(width, height, false);

    this.boardCamera.setAspect(width / height);
  }

  private startRenderLoop(): void {
    let previousTime: number | null = null;

    this.renderer.setAnimationLoop((time) => {
      const deltaSeconds = Math.min((time - (previousTime ?? time)) / 1000, 0.1);
      previousTime = time;

      this.boardCamera.update(deltaSeconds);
      this.hover.update(this.hoverables());
      this.updatePreview();
      this.renderer.render(this.scene, this.boardCamera.camera);
    });
  }

  // moves the pending tile onto the hovered empty place, hides it anywhere else
  private updatePreview(): void {
    const hovered = this.hover.current;
    const slot = this.pendingTile && hovered instanceof SlotView ? hovered : null;

    if (slot === this.previewSlot) {
      return;
    }

    this.previewSlot = slot;

    if (!slot || !this.pendingTile) {
      if (this.preview) {
        this.preview.object.visible = false;
      }

      return;
    }

    // the drawn tile has no place yet, it takes the coordinate of the hovered slot
    const tile: Tile = { ...this.pendingTile, coordinate: slot.coordinate };

    if (this.preview) {
      this.preview.update(tile);
    } else {
      this.preview = new TileView(tile, this.resources);
      this.scene.add(this.preview.object);
    }

    this.preview.setInvalid(!this.isValidPlacement(slot.coordinate));
    this.preview.object.visible = true;
  }

  // a tile can only be set next to a tile that is already on the board
  // a click on a free place the preview shows as valid puts the pending tile there
  private onBoardClick(target: Hoverable): void {
    if (!this.pendingTile || !(target instanceof SlotView)) {
      return;
    }

    if (this.isValidPlacement(target.coordinate)) {
      this.callbacks.onPlacePendingTile(target.coordinate);
    }
  }

  private isValidPlacement({ q, r }: HexCoordinate): boolean {
    return Object.values(EDGE_OFFSETS).some((offset) =>
      this.tileViews.has(coordinateKey({ q: q + offset.q, r: r + offset.r })),
    );
  }

  private hoverables(): Hoverable[] {
    const visibleSlots = [...this.slotViews.values()].filter((slot) => slot.object.visible);

    return [...this.tileViews.values(), ...visibleSlots];
  }

  private addSlots(): void {
    for (const coordinate of boardCoordinates()) {
      const view = new SlotView(coordinate, this.resources);
      this.slotViews.set(coordinateKey(coordinate), view);
      this.slots.add(view.object);
    }
  }

  private addLights(): void {
    this.scene.add(new HemisphereLight('#e9efe9', '#101b14', 1));

    // sun from the south east, the cel shading turns it into hard bands on the tiles
    const sun = new DirectionalLight('#ffffff', 2.5);
    sun.position.set(5, 10, 7);
    this.scene.add(sun);
  }
}
