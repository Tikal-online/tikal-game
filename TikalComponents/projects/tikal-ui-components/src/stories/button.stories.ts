import { applicationConfig, Meta, moduleMetadata, StoryObj } from '@storybook/angular-vite';
import { ButtonComponent } from '../lib/components/button/button';
import { ButtonColour } from '../lib/enums/button-colour';
import { ButtonSize } from '../lib/enums/button-size';

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
  },
  args: {
    colour: ButtonColour.Primary,
    size: ButtonSize.Normal,
    label: 'Button',
    icon: 'check',
    isLoading: false,
  },
  decorators: [
    applicationConfig({
      providers: [],
    }),
    moduleMetadata({
      declarations: [],
      imports: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<ButtonComponent>;

export const Primary: Story = {};
