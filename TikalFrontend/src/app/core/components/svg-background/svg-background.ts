import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-svg-background',
  imports: [],
  templateUrl: './svg-background.html',
  styleUrl: './svg-background.scss',
})
export class SvgBackgound {}
