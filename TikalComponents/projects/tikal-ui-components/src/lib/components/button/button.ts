import { Component, input, output } from '@angular/core';
import { PIcon, Spinner } from '@primeicons/angular';
import { ButtonModule } from 'primeng/button';
import { ButtonColour } from '../../enums/button-colour';
import { ButtonSize } from '../../enums/button-size';
import { EnumMapPipe } from '../../../pipes/enum-map';

@Component({
  selector: 'tikal-button',
  imports: [ButtonModule, PIcon, EnumMapPipe, Spinner],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class ButtonComponent {
  /** What icon should the button display? */
  readonly icon = input<string>('');

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

  /** Is the action related to the button currently running? */
  readonly isLoading = input<boolean>(false);

  /** Should the button have an outline? */
  readonly outline = input<boolean>(false);

  /** @ignore */
  readonly clicked = output<void>();
}
