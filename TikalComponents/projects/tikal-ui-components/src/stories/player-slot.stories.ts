import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { PlayerSlotComponent } from '../lib/components/player-slot/player-slot';

const meta: Meta<PlayerSlotComponent> = {
  title: 'Atoms/Player-Slot',
  component: PlayerSlotComponent,
  tags: ['autodocs'],
  args: {
    name: 'MyUser1234',
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<PlayerSlotComponent>;

export const PlayerSlot: Story = {
  name: 'Ready',
  args: {
    name: 'MyUser1234',
  },
};
