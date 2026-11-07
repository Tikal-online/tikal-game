import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ButtonComponent } from '../public-api';

const meta: Meta<ButtonComponent> = {
  title: 'Atoms/Button',
  component: ButtonComponent,
  tags: ['autodocs'],
  argTypes: {
    colour: {
      control: { type: 'select' },
    },
    size: {
      control: { type: 'select' },
    },
    type: {
      control: { type: 'select' },
    },
  },
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: 'Button',
    ariaLabel: 'Button',
    icon: 'check',
    isLoading: false,
    disabled: false,
    outline: false,
    text: false,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<ButtonComponent>;

export const PrimaryWithIcon: Story = {
  name: 'Icon and text',
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: 'Button',
    icon: 'check',
    isLoading: false,
    outline: false,
  },
};

export const TextOnly: Story = {
  name: 'Text only',
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: 'Button',
    icon: '',
    isLoading: false,
    outline: false,
  },
};

export const IconOnly: Story = {
  name: 'Icon only',
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: '',
    ariaLabel: 'Confirm',
    icon: 'check',
    isLoading: false,
    outline: false,
  },
};

export const Textoading: Story = {
  name: 'Text loading',
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: 'Button',
    icon: '',
    isLoading: true,
    outline: false,
  },
};

export const IconOnlyLoading: Story = {
  name: 'Icon only loading',
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: '',
    ariaLabel: 'Confirm',
    icon: 'check',
    isLoading: true,
    outline: false,
  },
};

export const TextButton: Story = {
  name: 'Text mode',
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: 'Button',
    icon: 'check',
    isLoading: false,
    outline: false,
    text: true,
  },
};

export const Disabled: Story = {
  name: 'Disabled',
  args: {
    colour: 'primary',
    size: 'normal',
    type: 'button',
    label: 'Button',
    icon: 'check',
    disabled: true,
  },
};

export const DangerOutlined: Story = {
  name: 'Danger outlined',
  args: {
    colour: 'danger',
    size: 'normal',
    type: 'button',
    label: 'Leave lobby',
    icon: '',
    outline: true,
  },
};
