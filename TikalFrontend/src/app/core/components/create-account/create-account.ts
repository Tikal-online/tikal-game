import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Menu } from '../menu/menu';
import { Button } from '../button/button';
import { form, maxLength, required, FormRoot, FormField } from '@angular/forms/signals';
import { AccountStore } from '../../stores/account-store/account-store';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingOverlay } from '../loading-overlay/loading-overlay';
import { translateSignal, TranslocoDirective } from '@jsverse/transloco';

type AccountData = {
  name: string;
};

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-create-account',
  imports: [Menu, Button, FormRoot, FormField, LoadingOverlay, TranslocoDirective],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccount {
  readonly accountData = signal<AccountData>({ name: '' });

  readonly accountForm = form(
    this.accountData,
    (schemaPath) => {
      required(schemaPath.name, { message: () => this.nameRequired() });
      maxLength(schemaPath.name, 30, { message: () => this.nameLength() });
    },
    {
      submission: {
        action: async (field) => {
          const name = field().value().name;

          const result = await this.accountStore.createAccount(name);

          if (result.isOk()) {
            const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';

            this.router.navigate([returnUrl], { replaceUrl: true });
            return;
          }

          return {
            kind: 'serverError',
            message: this.accountExists(),
            fieldTree: field.name,
          };
        },
      },
    },
  );

  private readonly accountStore = inject(AccountStore);

  private readonly router = inject(Router);

  private readonly route = inject(ActivatedRoute);

  // errors
  private readonly nameRequired = translateSignal('createAccount.errors.nameRequired');
  private readonly nameLength = translateSignal('createAccount.errors.nameLength', {
    maxLength: '30',
  });
  private readonly accountExists = translateSignal('createAccount.errors.accountExists');
}
