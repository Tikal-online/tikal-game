import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { PlayerSlotComponent } from '../public-api';

const meta: Meta<PlayerSlotComponent> = {
  title: 'Atoms/Player-Slot',
  component: PlayerSlotComponent,
  tags: ['autodocs'],
  argTypes: {
    colour: {
      control: { type: 'select' },
    },
  },
  args: {
    name: 'MyUser1234',
    colour: 'Red',
    isReady: false,
    isOwner: true,
    isMe: true,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<PlayerSlotComponent>;

export const Ready: Story = {
  name: 'Ready',
  args: {
    name: 'MyUser1234',
    colour: 'Red',
    isReady: true,
    isOwner: false,
    isMe: true,
  },
};

export const Owner: Story = {
  name: 'Owner',
  args: {
    name: 'MyUser1234',
    colour: 'Red',
    isReady: false,
    isOwner: true,
    isMe: false,
  },
};
