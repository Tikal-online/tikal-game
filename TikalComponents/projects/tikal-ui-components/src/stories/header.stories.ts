import { provideRouter, withHashLocation } from '@angular/router';
import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { HeaderComponent } from '../public-api';

// Hash routing keeps the router independent of Storybook's iframe URL: every story starts at "/".
const meta: Meta<HeaderComponent> = {
  title: 'Organisms/Header',
  component: HeaderComponent,
  tags: ['autodocs'],
  parameters: {
    layout: 'fullscreen',
  },
  decorators: [
    applicationConfig({
      providers: [provideRouter([{ path: '**', children: [] }], withHashLocation())],
    }),
  ],
  args: {
    homeRoute: '/',
    darkMode: false,
    links: [
      { label: 'Lobbies', route: '/', exact: true },
      { label: 'Profile', route: '/Profile' },
    ],
  },
};

export default meta;
type Story = StoryObj<HeaderComponent>;

export const Default: Story = {
  name: 'Default',
};
