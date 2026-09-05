import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ChatMessageComponent } from '../lib/components/chat-message/chat-message';

const meta: Meta<ChatMessageComponent> = {
  title: 'Atoms/Chat-Message',
  component: ChatMessageComponent,
  tags: ['autodocs'],
  args: {
    message: 'This is my test message',
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<ChatMessageComponent>;

export const oponentMessage: Story = {
  name: 'Opponent Message',
};
