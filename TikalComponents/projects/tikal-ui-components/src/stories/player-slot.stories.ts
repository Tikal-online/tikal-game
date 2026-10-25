import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { PlayerSlotComponent } from '../lib/components/player-slot/player-slot';

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
    colour: 'red',
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
    colour: 'red',
    isReady: true,
    isOwner: false,
    isMe: true,
  },
};

export const Owner: Story = {
  name: 'Owner',
  args: {
    name: 'MyUser1234',
    colour: 'red',
    isReady: false,
    isOwner: true,
    isMe: false,
  },
};
