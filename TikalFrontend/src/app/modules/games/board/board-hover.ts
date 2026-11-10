import { Camera, Mesh, Object3D, Raycaster, Vector2 } from 'three';

// a place on the board the mouse can point at
export type Hoverable = {
  // the solid part the mouse hits, outlines are left out
  readonly body: Mesh;

  setHighlighted(highlighted: boolean): void;
};

// highlights whatever place on the board the mouse is over
export class BoardHover {
  private readonly raycaster = new Raycaster();

  // mouse position in normalised device coordinates (-1 to 1), null while the mouse is not over the canvas
  private pointer: Vector2 | null = null;

  private hovered: Hoverable | null = null;

  constructor(
    private readonly canvas: HTMLCanvasElement,
    private readonly camera: Camera,
  ) {
    canvas.addEventListener('pointermove', this.onPointerMove);
    canvas.addEventListener('pointerleave', this.onPointerLeave);
  }

  // runs every frame, so the highlight also follows the board when the camera moves under a resting mouse
  update(candidates: readonly Hoverable[]): void {
    const next = this.pointer ? this.pick(this.pointer, candidates) : null;

    if (next === this.hovered) {
      return;
    }

    this.hovered?.setHighlighted(false);
    next?.setHighlighted(true);
    this.hovered = next;
  }

  dispose(): void {
    this.canvas.removeEventListener('pointermove', this.onPointerMove);
    this.canvas.removeEventListener('pointerleave', this.onPointerLeave);
  }

  // the nearest place under the mouse, places in front cover the ones behind
  private pick(pointer: Vector2, candidates: readonly Hoverable[]): Hoverable | null {
    const byBody = new Map<Object3D, Hoverable>(
      candidates.map((candidate) => [candidate.body, candidate]),
    );

    this.raycaster.setFromCamera(pointer, this.camera);
    const [nearest] = this.raycaster.intersectObjects([...byBody.keys()], false);

    return nearest ? (byBody.get(nearest.object) ?? null) : null;
  }

  private readonly onPointerMove = (event: PointerEvent): void => {
    // touch and pen have no hovering, a finger would leave its last place highlighted
    if (event.pointerType !== 'mouse') {
      this.pointer = null;
      return;
    }

    const rect = this.canvas.getBoundingClientRect();

    this.pointer = new Vector2(
      ((event.clientX - rect.left) / rect.width) * 2 - 1,
      -((event.clientY - rect.top) / rect.height) * 2 + 1,
    );
  };

  private readonly onPointerLeave = (): void => {
    this.pointer = null;
  };
}
