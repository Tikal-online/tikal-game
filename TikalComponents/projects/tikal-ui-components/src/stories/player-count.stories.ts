import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { PlayerCountComponent } from '../lib/components/player-count/player-count';

const meta: Meta<PlayerCountComponent> = {
  title: 'Atoms/Player-Count',
  component: PlayerCountComponent,
  tags: ['autodocs'],
  args: {
    currentPlayers: 2,
    maxPlayers: 4,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<PlayerCountComponent>;

export const HalfwayFull: Story = {
  name: 'Half full',
  args: {
    currentPlayers: 2,
    maxPlayers: 4,
  },
};

export const AlmostEmpty: Story = {
  name: 'Almost empty',
  args: {
    currentPlayers: 1,
    maxPlayers: 4,
  },
};

export const Full: Story = {
  name: 'Full',
  args: {
    currentPlayers: 3,
    maxPlayers: 3,
  },
};
