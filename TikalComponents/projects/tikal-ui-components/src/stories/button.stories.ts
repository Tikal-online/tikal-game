import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ButtonComponent } from '../lib/components/button/button';
import { ButtonColour } from '../lib/enums/button-colour';
import { ButtonSize } from '../lib/enums/button-size';
import { ButtonType } from '../lib/enums/button-type';

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
    colour: ButtonColour.Primary,
    size: ButtonSize.Normal,
    type: ButtonType.Button,
    label: 'Button',
    icon: 'check',
    isLoading: false,
    outline: false,
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
    colour: ButtonColour.Primary,
    size: ButtonSize.Normal,
    type: ButtonType.Button,
    label: 'Button',
    icon: 'check',
    isLoading: false,
    outline: false,
  },
};

export const TextOnly: Story = {
  name: 'Text only',
  args: {
    colour: ButtonColour.Primary,
    size: ButtonSize.Normal,
    type: ButtonType.Button,
    label: 'Button',
    icon: '',
    isLoading: false,
    outline: false,
  },
};

export const IconOnly: Story = {
  name: 'Icon only',
  args: {
    colour: ButtonColour.Primary,
    size: ButtonSize.Normal,
    type: ButtonType.Button,
    label: '',
    icon: 'check',
    isLoading: false,
    outline: false,
  },
};

export const Textoading: Story = {
  name: 'Text loading',
  args: {
    colour: ButtonColour.Primary,
    size: ButtonSize.Normal,
    type: ButtonType.Button,
    label: 'Button',
    icon: '',
    isLoading: true,
    outline: false,
  },
};

export const IconOnlyLoading: Story = {
  name: 'Icon only loading',
  args: {
    colour: ButtonColour.Primary,
    size: ButtonSize.Normal,
    type: ButtonType.Button,
    label: '',
    icon: 'check',
    isLoading: true,
    outline: false,
  },
};
