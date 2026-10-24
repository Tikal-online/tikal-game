import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { PlayerSlotListComponent } from '../lib/components/player-slot-list/player-slot-list';

const meta: Meta<PlayerSlotListComponent> = {
  title: 'Molecules/Player-Slot-List',
  component: PlayerSlotListComponent,
  tags: ['autodocs'],
  args: {
    maxPlayers: 4,
    isLoading: false,
    players: [
      {
        name: 'Player1',
        selectedColour: 'red',
        isReady: true,
      },
      {
        name: 'Player2',
        selectedColour: 'green',
        isReady: false,
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
    isLoading: false,
    players: [
      {
        name: 'Player1',
        selectedColour: 'red',
        isReady: false,
      },
      {
        name: 'Player2',
        selectedColour: 'green',
        isReady: true,
      },
    ],
  },
};

export const FullLobby: Story = {
  name: 'Full lobby',
  args: {
    maxPlayers: 3,
    isLoading: false,
    players: [
      {
        name: 'Player1',
        selectedColour: 'red',
        isReady: true,
      },
      {
        name: 'Player2',
        selectedColour: 'green',
        isReady: false,
      },
      {
        name: 'Player3',
        selectedColour: 'yellow',
        isReady: true,
      },
    ],
  },
};

export const EmptyLobby: Story = {
  name: 'Empty lobby',
  args: {
    maxPlayers: 4,
    isLoading: false,
    players: [],
  },
};

export const Loading: Story = {
  name: 'Loading',
  args: {
    maxPlayers: 4,
    isLoading: true,
    players: [],
  },
};
