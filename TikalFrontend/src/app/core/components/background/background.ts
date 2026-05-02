import {
  Component,
  ChangeDetectionStrategy,
  viewChild,
  ElementRef,
  AfterViewInit,
} from '@angular/core';
import { NgOptimizedImage } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-background',
  imports: [NgOptimizedImage, RouterOutlet],
  templateUrl: './background.html',
  styleUrl: './background.scss',
})
export class Background implements AfterViewInit {
  // coordinates of the torches
  private readonly bottomLeftCoordinates = {
    x: 0.282,
    y: 0.65,
  };

  private readonly bottomRightCoordinates = {
    x: 0.635,
    y: 0.68,
  };

  private readonly topLeftCoordinates = {
    x: 0.4325,
    y: 0.55,
  };

  private readonly topRightCoordinates = {
    x: 0.555,
    y: 0.55,
  };

  // element refs
  private readonly torchBottomLeft =
    viewChild.required<ElementRef<HTMLDivElement>>('torchBottomLeft');

  private readonly torchBottomRight =
    viewChild.required<ElementRef<HTMLDivElement>>('torchBottomRight');

  private readonly torchTopLeft = viewChild.required<ElementRef<HTMLDivElement>>('torchTopLeft');

  private readonly torchTopRight = viewChild.required<ElementRef<HTMLDivElement>>('torchTopRight');

  private readonly background = viewChild.required<ElementRef<HTMLImageElement>>('background');

  private readonly wrapper = viewChild.required<ElementRef<HTMLDivElement>>('backgroundContainer');

  ngAfterViewInit(): void {
    const imgElement = this.background().nativeElement;
    const wrapperElement = this.wrapper().nativeElement;

    const torchBottomLeft = {
      element: this.torchBottomLeft().nativeElement,
      coords: this.bottomLeftCoordinates,
    };

    const torchBottomRight = {
      element: this.torchBottomRight().nativeElement,
      coords: this.bottomRightCoordinates,
    };

    const torchTopLeft = {
      element: this.torchTopLeft().nativeElement,
      coords: this.topLeftCoordinates,
    };

    const torchTopRight = {
      element: this.torchTopRight().nativeElement,
      coords: this.topRightCoordinates,
    };

    const torches = [torchBottomLeft, torchBottomRight, torchTopLeft, torchTopRight];

    if (imgElement.complete) {
      this.positionTorches(imgElement, wrapperElement, torches);
    } else {
      imgElement.addEventListener('load', () =>
        this.positionTorches(imgElement, wrapperElement, torches),
      );
    }

    window.addEventListener('resize', () =>
      this.positionTorches(imgElement, wrapperElement, torches),
    );
  }

  private positionTorches(
    imgElement: HTMLImageElement,
    wrapperElement: HTMLDivElement,
    torches: {
      element: HTMLDivElement;
      coords: { x: number; y: number };
    }[],
  ): void {
    const naturalRatio = imgElement.naturalWidth / imgElement.naturalHeight;
    const boxRatio = wrapperElement.offsetWidth / wrapperElement.offsetHeight;

    let renderedWidth, renderedHeight, offsetX, offsetY;

    if (naturalRatio > boxRatio) {
      renderedHeight = wrapperElement.offsetHeight;
      renderedWidth = renderedHeight * naturalRatio;
      offsetX = (wrapperElement.offsetWidth - renderedWidth) / 2;
      offsetY = 0;
    } else {
      renderedWidth = wrapperElement.offsetWidth;
      renderedHeight = renderedWidth / naturalRatio;
      offsetX = 0;
      offsetY = (wrapperElement.offsetHeight - renderedHeight) / 2;
    }

    for (const torch of torches) {
      const torchElement = torch.element;

      const xPercent = torch.coords.x;
      const yPercent = torch.coords.y;

      const x = offsetX + xPercent * renderedWidth;
      const y = offsetY + yPercent * renderedHeight;

      if (x >= 0 && x <= wrapperElement.offsetWidth && y >= 0 && y <= wrapperElement.offsetHeight) {
        torchElement.style.left = `${x}px`;
        torchElement.style.top = `${y}px`;
        torchElement.style.display = 'block';
      } else {
        torchElement.style.display = 'none';
      }
    }
  }
}
