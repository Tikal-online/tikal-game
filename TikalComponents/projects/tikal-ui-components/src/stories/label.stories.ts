import { Meta, StoryObj } from '@storybook/angular-vite';
import { LabelComponent } from '../lib/components/label/label';

const meta: Meta<LabelComponent> = {
  title: 'Atoms/Label',
  component: LabelComponent,
  tags: ['autodocs'],
  args: {
    value: '',
    disabled: false,
    invalid: false,
    required: false,
    placeholder: 'Placeholder...',
  },
};

export default meta;
type Story = StoryObj<LabelComponent>;

export const EmptyWithPlaceholder: Story = {
  name: 'Empty with placeholder',
  args: {
    value: '',
    disabled: false,
    invalid: false,
    required: false,
    placeholder: 'Type something...',
  },
};
