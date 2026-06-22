import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideArrowLeft } from '@lucide/angular';
import { GlobalChat } from '../global-chat/global-chat';

@Component({
  selector: 'app-lobbies',
  imports: [RouterLink, LucideArrowLeft, GlobalChat],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class Lobbies {}
