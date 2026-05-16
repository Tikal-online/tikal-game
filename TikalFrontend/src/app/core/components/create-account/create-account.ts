import { ChangeDetectionStrategy, Component } from '@angular/core';
import { Menu } from '../menu/menu';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-create-account',
  imports: [Menu],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccount {}
