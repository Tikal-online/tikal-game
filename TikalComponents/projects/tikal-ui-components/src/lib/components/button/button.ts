import { Component, input } from '@angular/core';
import { PIcon } from '@primeicons/angular';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'tikal-button',
  imports: [ButtonModule, PIcon],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class ButtonComponent {
  readonly icon = input<string>('');
}
