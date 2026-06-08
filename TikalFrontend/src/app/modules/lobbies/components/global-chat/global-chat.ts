import { ChangeDetectionStrategy, Component, DestroyRef, inject } from '@angular/core';
import { GlobalChatStore } from '../../stores/global-chat/global-chat-store';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-global-chat',
  imports: [],
  templateUrl: './global-chat.html',
  styleUrl: './global-chat.scss',
})
export class GlobalChat {
  readonly globalChatStore = inject(GlobalChatStore);

  constructor() {
    this.globalChatStore.connect();
    inject(DestroyRef).onDestroy(() => this.globalChatStore.disconnect());
  }
}
