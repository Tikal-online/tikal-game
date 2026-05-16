import { ChangeDetectionStrategy, Component } from '@angular/core';
import { Menu } from '../menu/menu';
import { Button } from '../button/button';
import { ButtonStyle } from '../button/button-type';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-create-account',
  imports: [Menu, Button],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccount {
  readonly ButtonStyle = ButtonStyle;
}
