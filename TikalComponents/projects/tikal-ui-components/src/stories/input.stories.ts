import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { InputComponent } from '../lib/components/input/input';

const meta: Meta<InputComponent> = {
  title: 'Atoms/Input',
  component: InputComponent,
  tags: ['autodocs'],
  args: {
    value: '',
    disabled: false,
    invalid: false,
    required: false,
    touched: false,
    placeholder: 'Placeholder...',
    maxLength: 100,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<InputComponent>;

export const EmptyWithPlaceholder: Story = {
  name: 'Empty with placeholder',
  args: {
    value: '',
    disabled: false,
    invalid: false,
    required: false,
    touched: false,
    placeholder: 'Type something...',
    maxLength: 100,
  },
};

export const EmptyWithoutPlaceholder: Story = {
  name: 'Empty without placeholder',
  args: {
    value: '',
    disabled: false,
    invalid: false,
    required: false,
    touched: false,
    placeholder: '',
    maxLength: 100,
  },
};

export const WithContent: Story = {
  name: 'Holding value',
  args: {
    value: 'I typed this',
    disabled: false,
    invalid: false,
    required: false,
    touched: false,
    placeholder: 'Type something...',
    maxLength: 100,
  },
};

export const Disabled: Story = {
  name: 'Disabled',
  args: {
    value: '',
    disabled: true,
    invalid: false,
    required: false,
    touched: false,
    placeholder: 'Type something...',
    maxLength: 100,
  },
};

export const Invalid: Story = {
  name: 'Invalid with value',
  args: {
    value: 'This is invalid',
    disabled: false,
    invalid: true,
    required: false,
    touched: true,
    placeholder: 'Type something...',
    maxLength: 100,
  },
};

export const InvalidWithPlaceholder: Story = {
  name: 'Invalid with placeholder',
  args: {
    value: '',
    disabled: false,
    invalid: true,
    required: false,
    touched: true,
    placeholder: 'Type something...',
    maxLength: 100,
  },
};
