import { applicationConfig, Meta, StoryObj } from '@storybook/angular-vite';
import { SkeletonComponent } from '../lib/components/skeleton/skeleton';

const meta: Meta<SkeletonComponent> = {
  title: 'Atoms/Skeleton',
  component: SkeletonComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [],
    }),
  ],
};

export default meta;
type Story = StoryObj<SkeletonComponent>;

export const BaseSkeleton: Story = {
  name: 'Skeleton',
};
