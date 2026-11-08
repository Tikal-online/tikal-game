import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { PlayerCardComponent } from '../public-api';

const meta: Meta<PlayerCardComponent> = {
  title: 'Atoms/Player-Card',
  component: PlayerCardComponent,
  tags: ['autodocs'],
  argTypes: {
    colour: {
      control: { type: 'select' },
    },
  },
  args: {
    name: 'MyUser1234',
    colour: 'red',
    points: 12,
    isMe: false,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<PlayerCardComponent>;

export const Default: Story = {
  name: 'Default',
  args: {
    name: 'MyUser1234',
    colour: 'blue',
    points: 18,
    isMe: false,
  },
};

export const Me: Story = {
  name: 'Me',
  args: {
    name: 'MyUser1234',
    colour: 'red',
    points: 12,
    isMe: true,
  },
};

export const LongName: Story = {
  name: 'Long name',
  args: {
    name: 'AVeryLongPlayerNameThatKeepsGoing',
    colour: 'black',
    points: 140,
    isMe: true,
  },
};
