import { Meta, StoryObj } from '@storybook/angular-vite';
import { MaxPlayerSelectComponent } from '../lib/components/max-player-select/max-player-select';

const meta: Meta<MaxPlayerSelectComponent> = {
  title: 'Atoms/Max-Player-Select',
  component: MaxPlayerSelectComponent,
  tags: ['autodocs'],
  args: {
    value: 3,
    invalid: false,
  },
};

export default meta;
type Story = StoryObj<MaxPlayerSelectComponent>;

export const Valid: Story = {
  name: 'Valid',
  args: {
    value: 3,
    invalid: false,
  },
};

export const Invalid: Story = {
  name: 'Invalid',
  args: {
    value: 3,
    invalid: true,
  },
};
