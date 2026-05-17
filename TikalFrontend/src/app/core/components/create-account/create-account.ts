import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Menu } from '../menu/menu';
import { Button } from '../button/button';
import { ButtonStyle } from '../button/button-type';
import { form, maxLength, required, FormRoot, FormField } from '@angular/forms/signals';
import { AccountStore } from '../../stores/account-store/account-store';

type AccountData = {
  name: string;
};

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-create-account',
  imports: [Menu, Button, FormRoot, FormField],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccount {
  readonly ButtonStyle = ButtonStyle;

  readonly accountData = signal<AccountData>({ name: '' });

  readonly accountForm = form(this.accountData, (schemaPath) => {
    required(schemaPath.name, { message: 'Name is required' });
    maxLength(schemaPath.name, 30, { message: 'Name cannot exceed 30 characters' });
  });

  private readonly accountStore = inject(AccountStore);
}
