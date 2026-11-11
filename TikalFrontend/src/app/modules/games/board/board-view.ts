import {
  Box3,
  DirectionalLight,
  Group,
  HemisphereLight,
  MathUtils,
  PerspectiveCamera,
  Scene,
  Vector3,
  WebGLRenderer,
} from 'three';
import { MapControls } from 'three/addons/controls/MapControls.js';
import { Tile } from '../models/tile';
import { BoardHover, Hoverable } from './board-hover';
import { BoardKeyboardControls } from './board-keyboard-controls';
import { boardCoordinates } from './board-layout';
import { BoardResources, TILE_HEIGHT } from './board-resources';
import { boxCorners, fitDistance } from './camera-fit';
import { coordinateKey } from './hex-layout';
import { SlotView } from './slot-view';
import { TileView } from './tile-view';

// a narrow field of view keeps the perspective flat, the tiles in front look about as big as the ones behind
const FIELD_OF_VIEW = 30;

// the camera starts looking at the centre of the board from the south, 55° above the ground
const START_DIRECTION = new Vector3(
  0,
  Math.sin(MathUtils.degToRad(55)),
  Math.cos(MathUtils.degToRad(55)),
);

// space around the board, relative to its size on screen
const VIEW_MARGIN = 1.08;

// how close and how far the camera may move, relative to the distance that fits the whole board
const MIN_ZOOM_DISTANCE = 0.25;
const MAX_ZOOM_DISTANCE = 1.5;

// the camera stays above the board: from straight down to almost level with the table
const MAX_TILT = MathUtils.degToRad(85);

// longest step the movement makes at once, e.g. after the tab was in the background
const MAX_FRAME_SECONDS = 0.1;

export class BoardView {
  private readonly scene = new Scene();

  private readonly camera = new PerspectiveCamera(FIELD_OF_VIEW, 1, 0.1, 1000);

  private readonly renderer: WebGLRenderer;

  private readonly resources = new BoardResources();

  // Utils
  private readonly controls: MapControls;

  private readonly keyboardControls: BoardKeyboardControls;

  private readonly hover: BoardHover;

  // Placeholders
  private readonly slots = new Group();

  private readonly slotViews = new Map<string, SlotView>();

  // Tiles
  private readonly tiles = new Group();

  private readonly tileViews = new Map<string, TileView>();

  // the space the full board takes up, it stays the same while tiles are set
  private readonly boardBounds = new Box3();

  // the board and any tile set outside of it, for framing them and for keeping the view on the board
  private readonly bounds = new Box3();

  constructor(canvas: HTMLCanvasElement) {
    this.renderer = new WebGLRenderer({ canvas, antialias: true, alpha: true });
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

    this.controls = this.createControls(canvas);
    this.keyboardControls = new BoardKeyboardControls(this.controls, canvas);
    this.hover = new BoardHover(canvas, this.camera);

    this.scene.add(this.slots, this.tiles);
    this.addSlots();
    this.addLights();

    this.startRenderLoop();
    this.fitCamera();
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

    this.bounds.copy(this.boardBounds).union(new Box3().setFromObject(this.tiles));
  }

  // css size of the canvas
  resize(width: number, height: number): void {
    if (width === 0 || height === 0) {
      return;
    }

    // false: the canvas size is controlled by css, only the drawing buffer is resized
    this.renderer.setSize(width, height, false);

    this.camera.aspect = width / height;
    this.camera.updateProjectionMatrix();
  }

  dispose(): void {
    this.renderer.setAnimationLoop(null);

    this.keyboardControls.dispose();
    this.controls.dispose();
    this.hover.dispose();

    this.resources.dispose();
    this.renderer.dispose();
  }

  private startRenderLoop(): void {
    let previousTime: number | null = null;

    this.renderer.setAnimationLoop((time) => {
      const deltaSeconds = Math.min((time - (previousTime ?? time)) / 1000, MAX_FRAME_SECONDS);
      previousTime = time;

      this.keyboardControls.update(deltaSeconds);
      // applies the damping of the controls, the view glides out after the pointer is released
      this.controls.update();
      this.hover.update(this.hoverables());
      this.renderer.render(this.scene, this.camera);
    });
  }

  // every set tile and every place still without one
  private hoverables(): Hoverable[] {
    const visibleSlots = [...this.slotViews.values()].filter((slot) => slot.object.visible);

    return [...this.tileViews.values(), ...visibleSlots];
  }

  private createControls(canvas: HTMLCanvasElement): MapControls {
    const controls = new MapControls(this.camera, canvas);

    controls.enableDamping = true;
    controls.zoomToCursor = true;
    controls.maxPolarAngle = MAX_TILT;

    controls.addEventListener('change', () => this.keepViewOnBoard());

    return controls;
  }

  // the board always has its full size, every place starts as a slot until a tile is set on it
  private addSlots(): void {
    for (const coordinate of boardCoordinates()) {
      const view = new SlotView(coordinate, this.resources);
      this.slotViews.set(coordinateKey(coordinate), view);
      this.slots.add(view.object);
    }

    // as high as a set tile, so the framing does not change once tiles are set
    this.boardBounds.setFromObject(this.slots);
    this.boardBounds.max.y = TILE_HEIGHT;
    this.bounds.copy(this.boardBounds);
  }

  private addLights(): void {
    // soft light from the sky, darker from the jungle floor, keeps the shadowed sides from turning black
    this.scene.add(new HemisphereLight('#e9efe9', '#101b14', 1));

    // sun from the south east, the cel shading turns it into hard bands on the tiles
    const sun = new DirectionalLight('#ffffff', 2.5);
    sun.position.set(5, 10, 7);
    this.scene.add(sun);
  }

  // the point the camera looks at may not leave the board, so it cannot get lost in empty space
  private keepViewOnBoard(): void {
    if (this.bounds.isEmpty()) {
      return;
    }

    const { min, max } = this.bounds;
    const target = this.controls.target;

    const clamped = new Vector3(
      MathUtils.clamp(target.x, min.x, max.x),
      0,
      MathUtils.clamp(target.z, min.z, max.z),
    );

    // moves the camera along, so the view is only stopped and not turned
    this.camera.position.add(clamped.clone().sub(target));
    target.copy(clamped);
  }

  // looks at the centre of the board from the start direction, as close as possible with every corner in view
  private fitCamera(): void {
    if (this.bounds.isEmpty()) {
      return;
    }

    const target = this.bounds.getCenter(new Vector3()).setY(0);

    const distance = fitDistance(
      boxCorners(this.bounds),
      target,
      START_DIRECTION,
      MathUtils.degToRad(this.camera.fov),
      this.camera.aspect,
      VIEW_MARGIN,
    );

    this.camera.position.copy(START_DIRECTION).multiplyScalar(distance).add(target);
    this.camera.lookAt(target);

    this.controls.target.copy(target);
    this.controls.minDistance = distance * MIN_ZOOM_DISTANCE;
    this.controls.maxDistance = distance * MAX_ZOOM_DISTANCE;
    this.controls.update();
  }
}
