import { Box3, MathUtils, PerspectiveCamera, Vector3 } from 'three';
import { MapControls } from 'three/addons/controls/MapControls.js';
import { BoardKeyboardControls } from './board-keyboard-controls';
import { boxCorners, fitDistance } from './camera-fit';

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

// the camera looking at the board, moved by mouse, touch and keyboard
export class BoardCamera {
  readonly camera = new PerspectiveCamera(FIELD_OF_VIEW, 1, 0.1, 1000);

  // mouse and touch
  private readonly controls: MapControls;

  private readonly keyboardControls: BoardKeyboardControls;

  constructor(canvas: HTMLCanvasElement) {
    this.controls = new MapControls(this.camera, canvas);
    this.controls.enableDamping = true;
    this.controls.zoomToCursor = true;
    this.controls.maxPolarAngle = MAX_TILT;

    this.keyboardControls = new BoardKeyboardControls(this.controls, canvas);
  }

  // width / height of the canvas
  setAspect(aspect: number): void {
    this.camera.aspect = aspect;
    this.camera.updateProjectionMatrix();
  }

  // looks at the centre of the space from the start direction, as close as possible with every corner in view
  fit(space: Box3): void {
    const target = space.getCenter(new Vector3()).setY(0);

    const distance = fitDistance(
      boxCorners(space),
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

  // once per frame, before rendering
  update(deltaSeconds: number): void {
    this.keyboardControls.update(deltaSeconds);
    this.controls.update();
  }

  dispose(): void {
    this.keyboardControls.dispose();
    this.controls.dispose();
  }
}
