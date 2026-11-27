import { Camera, Object3D, Raycaster, Vector2 } from 'three';
import { Hoverable } from './board-hover';

// a press that moved further than this (css pixels) dragged the camera and is not a click
const MAX_CLICK_DISTANCE = 6;

// reports which place on the board was clicked or tapped, drags of the camera are left out
export class BoardClick {
  private readonly raycaster = new Raycaster();

  private press: { pointerId: number; x: number; y: number } | null = null;

  constructor(
    private readonly canvas: HTMLCanvasElement,
    private readonly camera: Camera,
    private readonly candidates: () => readonly Hoverable[],
    private readonly onClick: (target: Hoverable) => void,
  ) {
    canvas.addEventListener('pointerdown', this.onPointerDown);
    canvas.addEventListener('pointerup', this.onPointerUp);
    canvas.addEventListener('pointercancel', this.onPointerCancel);
  }

  dispose(): void {
    this.canvas.removeEventListener('pointerdown', this.onPointerDown);
    this.canvas.removeEventListener('pointerup', this.onPointerUp);
    this.canvas.removeEventListener('pointercancel', this.onPointerCancel);
  }

  private readonly onPointerDown = (event: PointerEvent): void => {
    // only the main button or a single finger, the other mouse buttons move the camera
    if (!event.isPrimary || event.button !== 0) {
      this.press = null;
      return;
    }

    this.press = { pointerId: event.pointerId, x: event.clientX, y: event.clientY };
  };

  private readonly onPointerUp = (event: PointerEvent): void => {
    const press = this.press;
    this.press = null;

    if (press?.pointerId !== event.pointerId) {
      return;
    }

    if (Math.hypot(event.clientX - press.x, event.clientY - press.y) > MAX_CLICK_DISTANCE) {
      return;
    }

    const target = this.pick(event);

    if (target) {
      this.onClick(target);
    }
  };

  private readonly onPointerCancel = (): void => {
    this.press = null;
  };

  // the nearest place under the pointer, places in front cover the ones behind
  private pick(event: PointerEvent): Hoverable | null {
    const rect = this.canvas.getBoundingClientRect();
    const pointer = new Vector2(
      ((event.clientX - rect.left) / rect.width) * 2 - 1,
      -((event.clientY - rect.top) / rect.height) * 2 + 1,
    );

    const byBody = new Map<Object3D, Hoverable>(
      this.candidates().map((candidate) => [candidate.body, candidate]),
    );

    this.raycaster.setFromCamera(pointer, this.camera);
    const [nearest] = this.raycaster.intersectObjects([...byBody.keys()], false);

    return nearest ? (byBody.get(nearest.object) ?? null) : null;
  }
}
