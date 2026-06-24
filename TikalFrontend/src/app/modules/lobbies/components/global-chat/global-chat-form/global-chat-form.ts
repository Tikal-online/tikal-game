import { Component, inject, signal } from '@angular/core';
import { GlobalChatStore } from '../../../stores/global-chat/global-chat-store';
import { form, required, maxLength, disabled, FormRoot, FormField } from '@angular/forms/signals';
import { TranslocoDirective } from '@jsverse/transloco';
import { LucideSendHorizontal } from '@lucide/angular';

type ChatFormData = {
  message: string;
};

@Component({
  selector: 'app-global-chat-form',
  imports: [TranslocoDirective, FormRoot, FormField, LucideSendHorizontal],
  templateUrl: './global-chat-form.html',
  styleUrl: './global-chat-form.scss',
})
export class GlobalChatForm {
  private readonly chatFormData = signal<ChatFormData>({ message: '' });

  private readonly globalChatStore = inject(GlobalChatStore);

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
}
