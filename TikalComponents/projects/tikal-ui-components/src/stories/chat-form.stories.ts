import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ChatFormComponent } from '../lib/components/chat-form/chat-form';

const meta: Meta<ChatFormComponent> = {
  title: 'Molecules/Chat-Form',
  component: ChatFormComponent,
  tags: ['autodocs'],
  args: {
    label: 'message',
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
    label: 'message',
    placeholder: 'Say something...',
    maxLength: 100,
  },
};

export const TypedContent: Story = {
  name: 'User typed content',
  args: {
    label: 'message',
    placeholder: 'Say something...',
    maxLength: 100,
  },
  play: async ({ canvas, userEvent }) => {
    const messageInput = canvas.getByLabelText('message', {
      selector: 'input',
    });

    await userEvent.type(messageInput, 'This is my message');
  },
};

export const Invalid: Story = {
  name: 'Submitted with empty content',
  args: {
    label: 'message',
    placeholder: 'Say something...',
    maxLength: 100,
  },
  play: async ({ canvas, userEvent }) => {
    const submitButton = canvas.getByRole('button');

    await userEvent.click(submitButton);
  },
};

export const Loading: Story = {
  name: 'Loading',
  args: {
    label: 'message',
    placeholder: 'Say something...',
    maxLength: 100,
    onSubmission: async () => {
      return new Promise(() => {
        /* never resolves */
      });
    },
  },
  play: async ({ canvas, userEvent }) => {
    const messageInput = canvas.getByLabelText('message', {
      selector: 'input',
    });

    await userEvent.type(messageInput, 'This is my message');

    const submitButton = canvas.getByRole('button');

    await userEvent.click(submitButton);
  },
};
