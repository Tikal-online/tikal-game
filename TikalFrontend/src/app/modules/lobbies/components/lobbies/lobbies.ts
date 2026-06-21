import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowLeft } from '@lucide/angular';

@Component({
  selector: 'app-lobbies',
  imports: [RouterLink, LucideArrowLeft],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class Lobbies {}
