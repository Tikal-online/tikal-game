import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ChatFormComponent } from '../lib/components/chat-form/chat-form';

const meta: Meta<ChatFormComponent> = {
  title: 'Molecules/Chat-Form',
  component: ChatFormComponent,
  tags: ['autodocs'],
  args: {
    placeholder: 'Say something...',
    maxLength: 100,
    onSubmission: async () => {
      return new Promise((resolve) => setTimeout(resolve, 5000));
    },
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<ChatFormComponent>;

export const WaitingForInput: Story = {
  name: 'Waiting for input',
  args: {
    placeholder: 'Say something...',
    maxLength: 100,
  },
};
