import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ChatMessageComponent } from '../lib/components/chat-message/chat-message';

const meta: Meta<ChatMessageComponent> = {
  title: 'Atoms/Chat-Message',
  component: ChatMessageComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<ChatMessageComponent>;

export const myMessage: Story = {
  name: 'My Message',
  args: {
    sender: 'Me',
    message: 'This was sent by me :)',
  },
};

export const opponentMessage: Story = {
  name: 'Opponent Message',
  args: {
    sender: 'Opponent',
    message: 'This was sent by my opponent',
  },
};
