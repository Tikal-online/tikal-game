import { Component, input, model } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FormValueControl } from '@angular/forms/signals';
import { SelectButtonModule } from 'primeng/selectbutton';

@Component({
  selector: 'tikal-max-player-select',
  imports: [SelectButtonModule, FormsModule],
  templateUrl: './max-player-select.html',
  styleUrl: './max-player-select.scss',
})
export class MaxPlayerSelectComponent implements FormValueControl<number> {
  /** What value should the input have? */
  readonly value = model(3);

  /** Does the control hold an invalid value? */
  readonly invalid = input(false);

  /** @ignore */
  readonly options = [
    { label: '2', value: 2 },
    { label: '3', value: 3 },
    { label: '4', value: 4 },
  ];
}
