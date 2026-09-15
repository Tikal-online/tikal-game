import { Component } from '@angular/core';
import { ButtonComponent, InputComponent } from 'tikal-ui-components';

@Component({
  selector: 'tikal-lobbies',
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
  imports: [ButtonComponent, InputComponent],
})
export class LobbiesComponent {}
