import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ChatMessageListComponent } from '../lib/components/chat-message-list/chat-message-list';

const meta: Meta<ChatMessageListComponent> = {
  title: 'Molecules/Chat-Message-List',
  component: ChatMessageListComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [],
    }),
    (story) => {
      const s = story();
      return {
        ...s,
        template: `<div style="display: flex; height: 500px; width: 400px;">${s.template}</div>`,
      };
    },
  ],
};

export default meta;
type Story = StoryObj<ChatMessageListComponent>;

export const multipleMessages: Story = {
  name: 'Multiple messages from different users',
  args: {
    messages: [
      { message: 'This is a message from another user', origin: 'Opponent' },
      { message: 'This is a message from me', origin: 'Me' },
      {
        message: 'This is a much much loooooooooooonger message from another user',
        origin: 'Opponent',
      },
      { message: 'Short', origin: 'Opponent' },
      { message: 'Me again :)', origin: 'Me' },
    ],
  },
};
