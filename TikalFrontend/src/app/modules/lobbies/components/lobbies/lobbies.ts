import { Component } from '@angular/core';
import { GlobalChat } from '../global-chat/global-chat';
import { LobbiesList } from '../lobbies-list/lobbies-list';

@Component({
  selector: 'app-lobbies',
  imports: [GlobalChat, LobbiesList],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class Lobbies {}
