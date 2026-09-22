import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { PlayerSlotListComponent } from '../lib/components/player-slot-list/player-slot-list';

const meta: Meta<PlayerSlotListComponent> = {
  title: 'Molecules/Player-Slot-List',
  component: PlayerSlotListComponent,
  tags: ['autodocs'],
  args: {
    maxPlayers: 4,
    players: [
      {
        name: 'Player1',
        colour: 'red',
      },
      {
        name: 'Player2',
        colour: 'green',
      },
    ],
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<PlayerSlotListComponent>;

export const HalfFullLobby: Story = {
  name: 'Half full lobby',
  args: {
    maxPlayers: 4,
    players: [
      {
        name: 'Player1',
        colour: 'red',
      },
      {
        name: 'Player2',
        colour: 'green',
      },
    ],
  },
};

export const FullLobby: Story = {
  name: 'Full lobby',
  args: {
    maxPlayers: 3,
    players: [
      {
        name: 'Player1',
        colour: 'red',
      },
      {
        name: 'Player2',
        colour: 'green',
      },
      {
        name: 'Player3',
        colour: 'yellow',
      },
    ],
  },
};

export const EmptyLobby: Story = {
  name: 'Empty lobby',
  args: {
    maxPlayers: 4,
    players: [],
  },
};
