import { provideRouter, withHashLocation } from '@angular/router';
import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { ActiveSessionLinkComponent } from '../public-api';

// Hash routing keeps the router independent of Storybook's iframe URL: every story starts at "/".
const meta: Meta<ActiveSessionLinkComponent> = {
  title: 'Atoms/Active Session Link',
  component: ActiveSessionLinkComponent,
  tags: ['autodocs'],
  argTypes: {
    type: {
      control: { type: 'select' },
      options: ['lobby', 'game'],
    },
  },
  decorators: [
    applicationConfig({
      providers: [provideRouter([{ path: '**', children: [] }], withHashLocation())],
    }),
  ],
  args: {
    name: 'name',
    route: '/Lobbies/me',
  },
};

export default meta;
type Story = StoryObj<ActiveSessionLinkComponent>;

export const Lobby: Story = {
  name: 'In a lobby',
  args: {
    type: 'lobby',
  },
};

export const Game: Story = {
  name: 'In a game',
  args: {
    type: 'game',
  },
};

export const Active: Story = {
  name: 'Active (current page)',
  args: {
    type: 'lobby',
    route: '/',
  },
};
