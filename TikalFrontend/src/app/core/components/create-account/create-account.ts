import { Component } from '@angular/core';
import { DialogModule } from 'primeng/dialog';
import { InputComponent, ButtonComponent } from 'tikal-ui-components';

@Component({
  selector: 'tikal-create-account',
  imports: [DialogModule, InputComponent, ButtonComponent],
  templateUrl: './create-account.html',
  styleUrl: './create-account.scss',
})
export class CreateAccountComponent {}
