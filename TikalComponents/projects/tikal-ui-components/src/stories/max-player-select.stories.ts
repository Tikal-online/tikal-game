import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { MaxPlayerSelectComponent } from '../lib/components/max-player-select/max-player-select';

const meta: Meta<MaxPlayerSelectComponent> = {
  title: 'Atoms/Max-Player-Select',
  component: MaxPlayerSelectComponent,
  tags: ['autodocs'],
  args: {
    value: 3,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<MaxPlayerSelectComponent>;

export const MaxPlayerSelect: Story = {
  name: 'Max Player Select',
  args: {
    value: 3,
  },
};
