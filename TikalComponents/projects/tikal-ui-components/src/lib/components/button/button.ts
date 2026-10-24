import { Component, input, output } from '@angular/core';
import { EnumMapPipe } from '../../../pipes/enum-map';
import { ButtonModule } from '@openng/optimus-ui/button';

export type ButtonColour = 'primary' | 'secondary';

export type ButtonSize = 'small' | 'normal' | 'large';

export type ButtonType = 'button' | 'submit' | 'reset';

@Component({
  selector: 'tikal-button',
  imports: [ButtonModule, EnumMapPipe],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class ButtonComponent {
  /** What icon should the button display? */
  readonly icon = input<string>('');

  /** What colour should the button be? */
  readonly colour = input<ButtonColour>('primary');

  /** What size should the button be? */
  readonly size = input<ButtonSize>('normal');

  /** @ignore */
  readonly sizeMap = {
    normal: undefined,
  };

  /** What text should the button display? */
  readonly label = input<string>('');

  /** Is the action related to the button currently running? */
  readonly isLoading = input<boolean>(false);

  /** Should the button have an outline? */
  readonly outline = input<boolean>(false);

  /** What type should the button be? */
  readonly type = input<ButtonType>('button');

  /** Should the button only appear as text? */
  readonly text = input<boolean>(false);

  /** @ignore */
  readonly clicked = output<void>();

  /** What form should the button belong to? */
  readonly form = input<string>();
}
