import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowLeft } from '@lucide/angular';

@Component({
  selector: 'app-create-lobby',
  imports: [LucideArrowLeft, RouterLink],
  templateUrl: './create-lobby.html',
  styleUrl: './create-lobby.scss',
})
export class CreateLobby {}
