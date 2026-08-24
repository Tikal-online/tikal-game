import { Component, model, input } from '@angular/core';
import { FormValueControl } from '@angular/forms/signals';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'tikal-input',
  imports: [InputTextModule],
  templateUrl: './input.html',
  styleUrl: './input.scss',
})
export class InputComponent implements FormValueControl<string> {
  /** What value should the input have */
  readonly value = model('');

  /** Should the input be disabled */
  readonly disabled = input<boolean>(false);

  /** Is the content of the input invalid? */
  readonly invalid = input<boolean>(false);

  /** Is the input required? */
  readonly required = input<boolean>(false);

  /** Was the input touched */
  readonly touched = input<boolean>(false);

  /** What is the max input length? */
  readonly maxLength = input<number>();

  /** What is the label of the input element? */
  readonly label = input<string>();

  /** What placeholder should the input display? */
  readonly placeholder = input<string>();
}
