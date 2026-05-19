import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Menu } from '../menu/menu';
import { Button } from '../button/button';
import { ButtonStyle } from '../button/button-type';
import { form, maxLength, required, FormRoot, FormField } from '@angular/forms/signals';
import { AccountStore } from '../../stores/account-store/account-store';
import { ActivatedRoute, Router } from '@angular/router';

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

  readonly accountForm = form(
    this.accountData,
    (schemaPath) => {
      required(schemaPath.name, { message: 'Name is required' });
      maxLength(schemaPath.name, 30, { message: 'Name cannot exceed 30 characters' });
    },
    {
      submission: {
        action: async (field) => {
          const name = field().value().name;

          const result = await this.accountStore.createAccount(name);

          if (result.isOk()) {
            this.router.navigate([this.returnUrl]);
            return;
          }

          return {
            kind: 'serverError',
            message: 'You already have an account. Please refresh this page',
            fieldTree: field.name,
          };
        },
      },
    },
  );

  private readonly accountStore = inject(AccountStore);

  private readonly router = inject(Router);

  private readonly route = inject(ActivatedRoute);

  private readonly returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
}
