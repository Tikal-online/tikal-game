import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { HintComponent } from '../public-api';

const meta: Meta<HintComponent> = {
  title: 'Atoms/Hint',
  component: HintComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
  args: {
    label: 'Click on the board to focus',
  },
};

export default meta;
type Story = StoryObj<HintComponent>;

export const Default: Story = {
  name: 'Default',
  args: {
    label: 'Click on the board to focus',
  },
};

export const LongText: Story = {
  name: 'Long text',
  args: {
    label:
      'Click on the board to focus it, then move the camera with W A S D and turn the drawn tile with T',
  },
};

export const InCorner: Story = {
  name: 'Placed in a corner',
  args: {
    label: 'Click on the board to focus',
  },
  render: (args) => ({
    props: args,
    template: `
      <div style="position: relative; height: 16rem; border-radius: 0.75rem; background: var(--p-surface-300);">
        <tikal-hint
          style="position: absolute; top: 1rem; left: 1rem; max-width: calc(100% - 2rem);"
          [label]="label"
        />
      </div>
    `,
  }),
};
