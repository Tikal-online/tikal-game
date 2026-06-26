import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowLeft } from '@lucide/angular';
import { GlobalChat } from '../global-chat/global-chat';
import { LobbiesList } from '../lobbies-list/lobbies-list';

@Component({
  selector: 'app-lobbies',
  imports: [RouterLink, LucideArrowLeft, GlobalChat, LobbiesList],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class Lobbies {}
