import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  ElementRef,
  inject,
  viewChild,
} from '@angular/core';
import { PerspectiveCamera, Scene, WebGLRenderer } from 'three';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'tikal-game-board',
  templateUrl: './game-board.html',
  styleUrl: './game-board.scss',
})
export class GameBoardComponent {
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  private readonly canvas = viewChild.required<ElementRef<HTMLCanvasElement>>('canvas');

  private readonly scene = new Scene();

  private readonly camera = new PerspectiveCamera(45, 1, 0.1, 1000);

  private renderer: WebGLRenderer | null = null;

  private resizeObserver: ResizeObserver | null = null;

  constructor() {
    // looking down onto the board from above at an angle
    this.camera.position.set(0, 10, 10);
    this.camera.lookAt(0, 0, 0);

    // WebGL needs the canvas in the DOM, so the renderer is only created after the first render
    afterNextRender(() => this.initRenderer());

    inject(DestroyRef).onDestroy(() => this.dispose());
  }

  private initRenderer(): void {
    const renderer = new WebGLRenderer({
      canvas: this.canvas().nativeElement,
      antialias: true,
      alpha: true,
    });

    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

    this.renderer = renderer;

    this.resizeObserver = new ResizeObserver(() => this.resize());
    this.resizeObserver.observe(this.host.nativeElement);
    this.resize();

    renderer.setAnimationLoop(() => renderer.render(this.scene, this.camera));
  }

  private resize(): void {
    if (!this.renderer) {
      return;
    }

    const { clientWidth, clientHeight } = this.host.nativeElement;

    if (clientWidth === 0 || clientHeight === 0) {
      return;
    }

    // false: the canvas size is controlled by css, only the drawing buffer is resized
    this.renderer.setSize(clientWidth, clientHeight, false);

    this.camera.aspect = clientWidth / clientHeight;
    this.camera.updateProjectionMatrix();
  }

  private dispose(): void {
    this.resizeObserver?.disconnect();

    this.renderer?.setAnimationLoop(null);
    this.renderer?.dispose();
  }
}
