import { MathUtils, PerspectiveCamera, Vector3 } from 'three';
import { MapControls } from 'three/addons/controls/MapControls.js';
import { BoardKeyboardControls } from './board-keyboard-controls';

// a narrow field of view keeps the perspective flat, the tiles in front look about as big as the ones behind
const FIELD_OF_VIEW = 30;

// the camera starts looking at the centre of the board from the south, 55° above the ground
const START_DIRECTION = new Vector3(
  0,
  Math.sin(MathUtils.degToRad(55)),
  Math.cos(MathUtils.degToRad(55)),
);

// the centre of the board, it always has the same size
const START_TARGET = new Vector3(5.25, 0, -3.03);

/**
 * Distances from START_TARGET at which the whole 8 × 5 board is in view, with about 8% space around it.
 * Narrow canvases are limited by the board's width (distance × aspect ratio), wide ones by its depth.
 * Measured for this board, field of view and start direction: recheck them when one of those changes.
 */
const FIT_WIDTH = 29.7;
const FIT_DEPTH = 18.4;

// how close and how far the camera may move, relative to the start distance
const MIN_ZOOM_DISTANCE = 0.25;
const MAX_ZOOM_DISTANCE = 1.5;

// the camera stays above the board: from straight down to almost level with the table
const MAX_TILT = MathUtils.degToRad(85);

// the camera looking at the board, moved by mouse, touch and keyboard
export class BoardCamera {
  readonly camera = new PerspectiveCamera(FIELD_OF_VIEW, 1, 0.1, 1000);

  private readonly controls: MapControls;

  private readonly keyboardControls: BoardKeyboardControls;

  constructor(canvas: HTMLCanvasElement) {
    this.controls = new MapControls(this.camera, canvas);
    this.controls.enableDamping = true;
    this.controls.zoomToCursor = true;
    this.controls.maxPolarAngle = MAX_TILT;

    this.keyboardControls = new BoardKeyboardControls(this.controls, canvas);
  }

  setAspect(aspect: number): void {
    this.camera.aspect = aspect;
    this.camera.updateProjectionMatrix();
  }

  reset(): void {
    const distance = Math.max(FIT_WIDTH / this.camera.aspect, FIT_DEPTH);

    this.camera.position.copy(START_DIRECTION).multiplyScalar(distance).add(START_TARGET);
    this.camera.lookAt(START_TARGET);

    this.controls.target.copy(START_TARGET);
    this.controls.minDistance = distance * MIN_ZOOM_DISTANCE;
    this.controls.maxDistance = distance * MAX_ZOOM_DISTANCE;
    this.controls.update();
  }

  update(deltaSeconds: number): void {
    this.keyboardControls.update(deltaSeconds);
    this.controls.update();
  }

  dispose(): void {
    this.keyboardControls.dispose();
    this.controls.dispose();
  }
}
