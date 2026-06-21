import { Component, inject, signal } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { form, maxLength, required, FormRoot, FormField, disabled } from '@angular/forms/signals';
import { AccountStore } from '../../stores/account-store/account-store';
import { LucideLoaderCircle } from '@lucide/angular';

type AccountData = {
  name: string;
};

@Component({
  selector: 'app-create-account',
  imports: [TranslocoDirective, RouterLink, FormRoot, FormField, LucideLoaderCircle],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccount {
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

          await new Promise((resolve) => setTimeout(resolve, 10000));

          const result = await this.accountStore.createAccount(name);

          if (result.isOk()) {
            const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';

            this.router.navigate([returnUrl], { replaceUrl: true });
            return;
          }

          return {
            kind: 'serverError',
            message: 'this is an error test',
            fieldTree: field.name,
          };
        },
      },
    },
  );

  private readonly accountStore = inject(AccountStore);

  private readonly router = inject(Router);

  private readonly route = inject(ActivatedRoute);
}
