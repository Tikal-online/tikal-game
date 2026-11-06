import { provideRouter, withHashLocation } from '@angular/router';
import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { NavLinkComponent } from '../public-api';

// Hash routing keeps the router independent of Storybook's iframe URL: every story starts at "/".
const meta: Meta<NavLinkComponent> = {
  title: 'Atoms/Nav Link',
  component: NavLinkComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [provideRouter([{ path: '**', children: [] }], withHashLocation())],
    }),
  ],
  args: {
    exact: true,
  },
};

export default meta;
type Story = StoryObj<NavLinkComponent>;

export const Active: Story = {
  name: 'Active (current page)',
  args: {
    label: 'Lobbies',
    route: '/',
  },
};

export const Inactive: Story = {
  name: 'Inactive',
  args: {
    label: 'Profile',
    route: '/Profile',
  },
};
