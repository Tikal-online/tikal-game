import { Component, input } from '@angular/core';
import { PIcon } from '@primeicons/angular';
import { ButtonModule } from 'primeng/button';
import { ButtonColour } from '../../enums/button-colour';
import { ButtonSize } from '../../enums/button-size';
import { EnumMapPipe } from '../../../pipes/enum-map';

@Component({
  selector: 'tikal-button',
  imports: [ButtonModule, PIcon, EnumMapPipe],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class ButtonComponent {
  /** What icon should the button display? */
  readonly icon = input<string>();

  /** What colour should the button be? */
  readonly colour = input<ButtonColour>(ButtonColour.Primary);

  /** What size should the button be? */
  readonly size = input<ButtonSize>(ButtonSize.Normal);

  /** @ignore */
  readonly sizeMap = {
    normal: undefined,
  };

  /** What text should the button display? */
  readonly label = input<string>();
}
