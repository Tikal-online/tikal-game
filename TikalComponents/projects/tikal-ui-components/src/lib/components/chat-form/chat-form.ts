import { Component, input, signal } from '@angular/core';
import { disabled, form, required, FormRoot, FormField } from '@angular/forms/signals';
import { InputComponent } from '../input/input';
import { ButtonComponent } from '../button/button';
import { ButtonType } from '../../enums/button-type';

type ChatFormData = {
  message: string;
};

@Component({
  selector: 'tikal-chat-form',
  imports: [FormRoot, FormField, InputComponent, ButtonComponent],
  templateUrl: './chat-form.html',
  styleUrl: './chat-form.scss',
})
export class ChatFormComponent {
  /** @ignore */
  private readonly chatFormData = signal<ChatFormData>({ message: '' });

  /** What should happen when the form is submitted? */
  readonly onSubmission = input.required<(message: string) => Promise<void>>();

  /** @ignore */
  readonly buttonTypeSubmit = ButtonType.Submit;

  /** @ignore */
  readonly chatForm = form(
    this.chatFormData,
    (schemaPath) => {
      required(schemaPath.message);
      disabled(schemaPath, { when: () => this.chatForm().submitting() });
    },
    {
      submission: {
        action: async (field) => {
          const message = field().value().message;

          await this.onSubmission()(message);

          field.message().reset('');
        },
      },
    },
  );
}
