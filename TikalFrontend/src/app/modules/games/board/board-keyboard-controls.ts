import { MathUtils, Spherical, Vector3 } from 'three';
import { MapControls } from 'three/addons/controls/MapControls.js';

type Action =
  | 'forward'
  | 'back'
  | 'left'
  | 'right'
  | 'turnLeft'
  | 'turnRight'
  | 'tiltUp'
  | 'tiltDown'
  | 'zoomIn'
  | 'zoomOut';

const KEY_ACTIONS: Readonly<Record<string, Action>> = {
  KeyW: 'forward',
  ArrowUp: 'forward',
  KeyS: 'back',
  ArrowDown: 'back',
  KeyA: 'left',
  ArrowLeft: 'left',
  KeyD: 'right',
  ArrowRight: 'right',
  KeyQ: 'turnLeft',
  KeyE: 'turnRight',
  KeyR: 'tiltUp',
  KeyF: 'tiltDown',
  Equal: 'zoomIn',
  NumpadAdd: 'zoomIn',
  Minus: 'zoomOut',
  NumpadSubtract: 'zoomOut',
};

// share of the distance to the board moved per second, zoomed out it moves faster
const MOVE_SPEED = 0.8;

// radians per second
const TURN_SPEED = MathUtils.degToRad(90);
const TILT_SPEED = MathUtils.degToRad(60);

// factor the distance changes by per second
const ZOOM_SPEED = 1.8;

export class BoardKeyboardControls {
  private readonly pressed = new Set<Action>();

  constructor(
    private readonly controls: MapControls,
    private readonly canvas: HTMLCanvasElement,
  ) {
    canvas.addEventListener('keydown', this.onKeyDown);
    canvas.addEventListener('keyup', this.onKeyUp);
    canvas.addEventListener('blur', this.onBlur);
    // a click on the board gives it the focus, so the keys work right after using the mouse
    canvas.addEventListener('pointerdown', this.onPointerDown);
  }

  update(deltaSeconds: number): void {
    if (this.pressed.size === 0) {
      return;
    }

    const camera = this.controls.object;
    const target = this.controls.target;

    const offset = camera.position.clone().sub(target);
    const spherical = new Spherical().setFromVector3(offset);

    // moving: along the ground, up and right on the screen projected onto the board
    const screenUp = new Vector3(0, 1, 0).applyQuaternion(camera.quaternion).setY(0).normalize();
    const screenRight = new Vector3(1, 0, 0).applyQuaternion(camera.quaternion).setY(0).normalize();

    const move = new Vector3()
      .addScaledVector(screenUp, this.axis('forward', 'back'))
      .addScaledVector(screenRight, this.axis('right', 'left'));

    if (move.lengthSq() > 0) {
      move.normalize().multiplyScalar(MOVE_SPEED * spherical.radius * deltaSeconds);
      target.add(move);
    }

    // turning around the vertical axis through the point looked at
    spherical.theta += this.axis('turnRight', 'turnLeft') * TURN_SPEED * deltaSeconds;

    // tilting: the polar angle is measured from straight above, a bigger one is a flatter view
    spherical.phi = MathUtils.clamp(
      spherical.phi + this.axis('tiltDown', 'tiltUp') * TILT_SPEED * deltaSeconds,
      this.controls.minPolarAngle,
      this.controls.maxPolarAngle,
    );

    spherical.radius = MathUtils.clamp(
      spherical.radius * Math.pow(ZOOM_SPEED, this.axis('zoomOut', 'zoomIn') * deltaSeconds),
      this.controls.minDistance,
      this.controls.maxDistance,
    );

    spherical.makeSafe();

    camera.position.copy(target).add(offset.setFromSpherical(spherical));
    this.controls.update();
  }

  dispose(): void {
    this.canvas.removeEventListener('keydown', this.onKeyDown);
    this.canvas.removeEventListener('keyup', this.onKeyUp);
    this.canvas.removeEventListener('blur', this.onBlur);
    this.canvas.removeEventListener('pointerdown', this.onPointerDown);
  }

  // 1 while only the positive key is held, -1 for only the negative one, 0 for none or both
  private axis(positive: Action, negative: Action): number {
    return Number(this.pressed.has(positive)) - Number(this.pressed.has(negative));
  }

  private readonly onKeyDown = (event: KeyboardEvent): void => {
    // leaves browser shortcuts like ctrl + minus alone
    if (event.ctrlKey || event.metaKey || event.altKey) {
      return;
    }

    const action = KEY_ACTIONS[event.code];

    if (!action) {
      return;
    }

    // arrows would scroll the page
    event.preventDefault();

    this.pressed.add(action);
  };

  private readonly onKeyUp = (event: KeyboardEvent): void => {
    const action = KEY_ACTIONS[event.code];

    if (action) {
      this.pressed.delete(action);
    }
  };

  // keys released while the focus is elsewhere would otherwise keep the camera moving
  private readonly onBlur = (): void => {
    this.pressed.clear();
  };

  private readonly onPointerDown = (): void => {
    this.canvas.focus({ preventScroll: true });
  };
}
