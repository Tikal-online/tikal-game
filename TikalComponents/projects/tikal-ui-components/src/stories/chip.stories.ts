import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ChipComponent } from '../public-api';

const meta: Meta<ChipComponent> = {
  title: 'Atoms/Chip',
  component: ChipComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<ChipComponent>;

export const WithIcon: Story = {
  name: 'Chip with icon',
  args: {
    icon: 'check',
    label: 'Chip Component',
  },
};

export const TextOnly: Story = {
  name: 'Chip without icon',
  args: {
    label: 'Chip Component',
  },
};
