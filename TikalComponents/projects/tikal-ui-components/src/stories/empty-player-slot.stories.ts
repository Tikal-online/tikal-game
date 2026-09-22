import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { EmptyPlayerSlotComponent } from '../lib/components/empty-player-slot/empty-player-slot';

const meta: Meta<EmptyPlayerSlotComponent> = {
  title: 'Atoms/Empty-Player-Slot',
  component: EmptyPlayerSlotComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<EmptyPlayerSlotComponent>;

export const EmptyPlayerSlot: Story = {
  name: 'Empty player slot',
};
