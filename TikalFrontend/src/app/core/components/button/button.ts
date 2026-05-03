import { Component, input, output } from '@angular/core';
import { ButtonStyle } from './button-type';

@Component({
  selector: 'app-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
  readonly ButtonStyle = ButtonStyle;

  readonly style = input<ButtonStyle>(ButtonStyle.Primary);

  readonly disabled = input<boolean>(false);

  readonly type = input<string>('button');

  readonly form = input<string>('');

  readonly clicked = output();

  onClicked(): void {
    this.clicked.emit();
  }
}
