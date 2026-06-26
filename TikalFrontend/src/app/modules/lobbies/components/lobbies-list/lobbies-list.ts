import { Component } from '@angular/core';
import { LucideGlobe, LucideUsers } from '@lucide/angular';

@Component({
  selector: 'app-lobbies-list',
  imports: [LucideGlobe, LucideUsers],
  templateUrl: './lobbies-list.html',
  styleUrl: './lobbies-list.scss',
})
export class LobbiesList {}
