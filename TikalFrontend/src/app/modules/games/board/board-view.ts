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

  private readonly controls: MapControls;

  // Shared GPU resources
  private readonly resources = new BoardResources();

  // Utils
  private readonly keyboardControls: BoardKeyboardControls;

  private readonly hover: BoardHover;

  // Placeholders
  private readonly slots = new Group();

  private readonly slotViews = new Map<string, SlotView>();

  // Tiles
  private readonly tiles = new Group();

  private readonly tileViews = new Map<string, TileView>();

  // the drawing buffer follows the canvas' css size
  private readonly resizeObserver = new ResizeObserver(() => this.resize());

  constructor(canvas: HTMLCanvasElement) {
    this.renderer = new WebGLRenderer({ canvas, antialias: true, alpha: true });
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

    this.controls = this.createControls(canvas);
    this.keyboardControls = new BoardKeyboardControls(this.controls, canvas);
    this.hover = new BoardHover(canvas, this.camera);

    this.scene.add(this.slots, this.tiles);
    this.addSlots();
    this.addLights();

    this.resize();
    this.fitCamera();
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

  dispose(): void {
    this.renderer.setAnimationLoop(null);
    this.resizeObserver.disconnect();

    this.keyboardControls.dispose();
    this.controls.dispose();
    this.hover.dispose();

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

    this.camera.aspect = width / height;
    this.camera.updateProjectionMatrix();
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

  private hoverables(): Hoverable[] {
    const visibleSlots = [...this.slotViews.values()].filter((slot) => slot.object.visible);

    return [...this.tileViews.values(), ...visibleSlots];
  }

  private createControls(canvas: HTMLCanvasElement): MapControls {
    const controls = new MapControls(this.camera, canvas);

    controls.enableDamping = true;
    controls.zoomToCursor = true;
    controls.maxPolarAngle = MAX_TILT;

    return controls;
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

  // looks at the centre of the board from the start direction, as close as possible with every corner in view
  private fitCamera(): void {
    const board = new Box3().setFromObject(this.slots);
    board.max.y = TILE_HEIGHT;

    const target = board.getCenter(new Vector3()).setY(0);

    const distance = fitDistance(
      boxCorners(board),
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
