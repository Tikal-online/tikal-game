import { Component, DestroyRef, inject, signal } from '@angular/core';
import { LucideMessageSquare, LucideSendHorizontal } from '@lucide/angular';
import { disabled, form, FormField, FormRoot, maxLength, required } from '@angular/forms/signals';
import { GlobalChatStore } from '../../stores/global-chat/global-chat-store';
import { TranslocoDirective } from '@jsverse/transloco';
import { AuthStore } from '../../../../core/stores/auth-store/auth-store';
import { ChatMessage } from '../../services/global-chat/global-chat-service';
import { GlobalChatHeader } from './global-chat-header/global-chat-header';
import { GlobalChatMessages } from './global-chat-messages/global-chat-messages';

type ChatFormData = {
  message: string;
};

@Component({
  selector: 'app-global-chat',
  imports: [
    LucideMessageSquare,
    LucideSendHorizontal,
    FormRoot,
    FormField,
    TranslocoDirective,
    GlobalChatHeader,
    GlobalChatMessages,
  ],
  templateUrl: './global-chat.html',
  styleUrl: './global-chat.scss',
})
export class GlobalChat {
  private readonly authStore = inject(AuthStore);

  private readonly chatFormData = signal<ChatFormData>({ message: '' });

  readonly globalChatStore = inject(GlobalChatStore);

  readonly chatForm = form(
    this.chatFormData,
    (schemaPath) => {
      required(schemaPath.message);
      maxLength(schemaPath.message, 200);
      disabled(schemaPath, { when: () => this.chatForm().submitting() });
    },
    {
      submission: {
        action: async (field) => {
          const message = field().value().message;

          await this.globalChatStore.sendMessage(message);

          field.message().reset('');

          // TODO: find a better way to give the message input focus after form submission
          setTimeout(() => {
            this.chatForm.message().focusBoundControl();
          }, 10);
        },
      },
    },
  );

  constructor() {
    this.globalChatStore.connect();
    inject(DestroyRef).onDestroy(() => this.globalChatStore.disconnect());
  }

  isOwnMessage(message: ChatMessage): boolean {
    return message.userId === this.authStore.userId();
  }
}
