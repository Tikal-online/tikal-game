import { Player } from './player';

export const PLAYER_TESTCASES: Player[] = [
  {
    userId: 'f47ac10b-58cc-4372-a567-0e02b2c3d479',
    name: 'Alice',
    isOwner: true,
    isReady: true,
  },
  {
    userId: '9b2e1c4a-3f6d-4a8b-9c1e-2d5f7a8b9c0d',
    name: 'Bob',
    isOwner: false,
    isReady: false,
  },
  {
    userId: 'c3d4e5f6-7a8b-49c0-9d1e-2f3a4b5c6d7e',
    name: 'Charlie',
    isOwner: false,
    isReady: true,
  },
  {
    userId: 'a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d',
    name: 'Dana',
    isOwner: true,
    isReady: false,
  },
  {
    userId: '7e8f9a0b-1c2d-4e3f-9a0b-1c2d3e4f5a6b',
    name: 'Zoë Müller',
    isOwner: false,
    isReady: true,
  },
  {
    userId: '2f3e4d5c-6b7a-4890-8c1d-2e3f4a5b6c7d',
    name: 'X',
    isOwner: false,
    isReady: false,
  },
  {
    userId: '5d6c7b8a-9e0f-4a1b-2c3d-4e5f6a7b8c9d',
    name: 'A Very Long Player Name That Might Break The UI Layout',
    isOwner: false,
    isReady: true,
  },
];
