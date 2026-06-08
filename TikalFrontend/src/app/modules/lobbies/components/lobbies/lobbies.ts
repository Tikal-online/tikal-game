import { Component, ChangeDetectionStrategy } from '@angular/core';
import { TranslocoDirective } from '@jsverse/transloco';
import { GlobalChat } from '../global-chat/global-chat';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-lobbies-page',
  imports: [TranslocoDirective, GlobalChat],
  templateUrl: './lobbies.html',
  styleUrl: './lobbies.scss',
})
export class LobbiesPage {}
