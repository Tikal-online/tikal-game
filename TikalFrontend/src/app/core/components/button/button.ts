import { Component, input, output } from '@angular/core';

export type ButtonVariant = 'primary' | 'error';

@Component({
  selector: 'app-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
  readonly variant = input<ButtonVariant>('primary');

  readonly disabled = input<boolean>(false);

  readonly type = input<string>('button');

  readonly clicked = output();

  onClicked(): void {
    this.clicked.emit();
  }
}
