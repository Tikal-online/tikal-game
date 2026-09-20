import { Component, inject, signal } from '@angular/core';
import { disabled, form, maxLength, required, FormRoot, FormField } from '@angular/forms/signals';
import { DialogModule } from 'primeng/dialog';
import { InputComponent, ButtonComponent } from 'tikal-ui-components';
import { AccountStore } from '../../stores/account-store/account-store';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslocoDirective } from '@jsverse/transloco';

type AccountData = {
  name: string;
};

@Component({
  selector: 'tikal-create-account',
  imports: [DialogModule, InputComponent, ButtonComponent, FormRoot, FormField, TranslocoDirective],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccountComponent {
  private readonly accountStore = inject(AccountStore);

  private readonly router = inject(Router);

  private readonly route = inject(ActivatedRoute);

  private readonly accountData = signal<AccountData>({ name: '' });

  readonly accountForm = form(
    this.accountData,
    (schemaPath) => {
      required(schemaPath.name);
      maxLength(schemaPath.name, 30);
      disabled(schemaPath, { when: () => this.accountForm().submitting() });
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
            fieldTree: field.name,
          };
        },
      },
    },
  );
}
