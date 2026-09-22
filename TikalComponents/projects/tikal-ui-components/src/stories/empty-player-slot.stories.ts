import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { EmptyPlayerSlotComponent } from '../lib/components/empty-player-slot/empty-player-slot';

const meta: Meta<EmptyPlayerSlotComponent> = {
  title: 'Atoms/Empty-Player-Slot',
  component: EmptyPlayerSlotComponent,
  tags: ['autodocs'],
  args: {
    isLoading: false,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<EmptyPlayerSlotComponent>;

export const Loaded: Story = {
  name: 'Loaded',
  args: {
    isLoading: false,
  },
};

export const Loading: Story = {
  name: 'Loading',
  args: {
    isLoading: true,
  },
};
