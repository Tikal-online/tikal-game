import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { TurnStatusComponent } from '../public-api';

const meta: Meta<TurnStatusComponent> = {
  title: 'Molecules/Turn-Status',
  component: TurnStatusComponent,
  tags: ['autodocs'],
  argTypes: {
    turnPhase: {
      control: { type: 'select' },
      options: ['PlaceTile', 'Actions'],
    },
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
  args: {
    turnPhase: 'PlaceTile',
  },
};

export default meta;
type Story = StoryObj<TurnStatusComponent>;

export const PlaceTile: Story = {
  name: 'Placing a tile',
  args: {
    turnPhase: 'PlaceTile',
  },
};

export const Actions: Story = {
  name: 'Doing actions',
  args: {
    turnPhase: 'Actions',
  },
};
