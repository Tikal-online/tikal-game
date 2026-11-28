import { DirectionalLight, Group, HemisphereLight, Scene, WebGLRenderer } from 'three';
import { HexCoordinate, Tile } from '../models/tile';
import { BoardCamera } from './utils/board-camera';
import { BoardClick } from './utils/board-click';
import { BoardHover, Hoverable } from './utils/board-hover';
import { boardCoordinates } from './board-layout';
import { BoardResources } from './board-resources';
import { coordinateKey } from './hex-layout';
import { SlotView } from './slot-view';
import { TileView } from './tile-view';

// how the board reports what the user does on it
export type BoardViewCallbacks = {
  /** The mouse moved onto another free place, the pending tile should follow it. */
  onHoveredPlaceChanged: (coordinate: HexCoordinate) => void;

  /** A free place was clicked. */
  onPlaceClicked: (coordinate: HexCoordinate) => void;
};

export class BoardView {
  private readonly scene = new Scene();

  private readonly renderer: WebGLRenderer;

  // Shared GPU resources
  private readonly resources = new BoardResources();

  // Utils
  private readonly camera: BoardCamera;

  private readonly hover: BoardHover;

  private readonly click: BoardClick;

  // Placeholders
  private readonly slots = new Group();

  private readonly slotViews = new Map<string, SlotView>();

  // Tiles
  private readonly tiles = new Group();

  private readonly tileViews = new Map<string, TileView>();

  // Active turn helpers
  private hoveredSlot: SlotView | null | undefined = undefined;

  private preview: TileView | null = null;

  // the drawing buffer follows the canvas' css size
  private readonly resizeObserver = new ResizeObserver(() => this.resize());

  constructor(
    canvas: HTMLCanvasElement,
    private readonly callbacks: BoardViewCallbacks,
  ) {
    this.renderer = new WebGLRenderer({ canvas, antialias: true, alpha: true });
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

    this.camera = new BoardCamera(canvas);
    this.hover = new BoardHover(canvas, this.camera.camera);
    this.click = new BoardClick(
      canvas,
      this.camera.camera,
      () => this.hoverables(),
      (target) => this.onBoardClick(target),
    );

    this.scene.add(this.slots, this.tiles);
    this.addSlots();
    this.addLights();

    this.resize();
    this.camera.reset();
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

  setPendingTile(tile: Tile | null, invalid: boolean): void {
    if (!tile) {
      if (this.preview) {
        this.scene.remove(this.preview.object);
        this.preview = null;
      }

      return;
    }

    // a newly drawn tile does not know the place under a resting mouse yet
    const hovered = this.hoveredSlot?.coordinate;

    if (hovered && (tile.coordinate.q !== hovered.q || tile.coordinate.r !== hovered.r)) {
      this.callbacks.onHoveredPlaceChanged(hovered);
    }

    if (this.preview) {
      this.preview.update(tile);
    } else {
      this.preview = new TileView(tile, this.resources);
      this.scene.add(this.preview.object);
    }

    this.preview.setInvalid(invalid);
  }

  dispose(): void {
    this.renderer.setAnimationLoop(null);
    this.resizeObserver.disconnect();

    this.camera.dispose();
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

    this.camera.setAspect(width / height);
  }

  private startRenderLoop(): void {
    let previousTime: number | null = null;

    this.renderer.setAnimationLoop((time) => {
      const deltaSeconds = Math.min((time - (previousTime ?? time)) / 1000, 0.1);
      previousTime = time;

      this.camera.update(deltaSeconds);
      this.hover.update(this.hoverables());
      this.reportHoveredPlace();
      this.renderer.render(this.scene, this.camera.camera);
    });
  }

  private reportHoveredPlace(): void {
    const hovered = this.hover.current;
    const slot = hovered instanceof SlotView ? hovered : null;

    if (slot === this.hoveredSlot) {
      return;
    }

    this.hoveredSlot = slot;

    if (slot) {
      this.callbacks.onHoveredPlaceChanged(slot.coordinate);
    }
  }

  private onBoardClick(target: Hoverable): void {
    if (target instanceof SlotView) {
      this.callbacks.onPlaceClicked(target.coordinate);
    }
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
