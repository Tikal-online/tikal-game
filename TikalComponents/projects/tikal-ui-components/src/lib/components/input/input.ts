import {
  Component,
  model,
  input,
  InputSignal,
  InputSignalWithTransform,
  OutputRef,
} from '@angular/core';
import {
  DisabledReason,
  FormValueControl,
  ValidationError,
  WithOptionalFieldTree,
} from '@angular/forms/signals';
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

  /*
  pending?: InputSignal<boolean> | InputSignalWithTransform<boolean, unknown> | undefined;
  name?: InputSignal<string> | InputSignalWithTransform<string, unknown> | undefined;
  minLength?:
    | InputSignal<number | undefined>
    | InputSignalWithTransform<number | undefined, unknown>
    | undefined;
  maxLength?:
    | InputSignal<number | undefined>
    | InputSignalWithTransform<number | undefined, unknown>
    | undefined;
  /** What placeholder should the input display? */
  readonly placeholder = input<string>();
}
