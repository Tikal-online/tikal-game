import { applicationConfig, Meta, moduleMetadata, StoryObj } from '@storybook/angular-vite';
import { ButtonComponent } from '../lib/components/button/button';

const meta: Meta<ButtonComponent> = {
  title: 'Atoms/Button',
  component: ButtonComponent,
  tags: ['autodocs'],
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
