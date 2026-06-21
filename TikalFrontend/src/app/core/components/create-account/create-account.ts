import { Component } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-create-account',
  imports: [TranslocoDirective, RouterLink],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccount {}
