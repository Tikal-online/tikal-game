import { Meta, StoryObj } from '@storybook/angular-vite';
import { CountdownOverlayComponent } from '../public-api';

const meta: Meta<CountdownOverlayComponent> = {
  title: 'Organisms/Countdown-Overlay',
  component: CountdownOverlayComponent,
  tags: ['autodocs'],
  parameters: {
    layout: 'fullscreen',
  },
  // the overlay is fixed to the viewport, on the docs page a transformed wrapper becomes its viewport
  render: (args, { viewMode }) => ({
    props: { ...args, inDocs: viewMode === 'docs' },
    template: `
      <div [style.transform]="inDocs ? 'translateZ(0)' : null" [style.height]="inDocs ? '25rem' : null">
        <tikal-countdown-overlay [finalLabel]="finalLabel" [label]="label" [secondsLeft]="secondsLeft" />
      </div>
    `,
  }),
  argTypes: {
    secondsLeft: {
      control: { type: 'number', min: 0, max: 10, step: 1 },
    },
  },
  args: {
    secondsLeft: 3,
    label: 'Game starts in',
    finalLabel: 'Starting game…',
  },
};

export default meta;
type Story = StoryObj<CountdownOverlayComponent>;

export const Counting: Story = {
  name: 'Counting down',
  args: {
    secondsLeft: 3,
  },
};

export const LastSecond: Story = {
  name: 'Last second',
  args: {
    secondsLeft: 1,
  },
};

export const Starting: Story = {
  name: 'Starting',
  args: {
    secondsLeft: 0,
  },
};
