import { Lobby } from '../../models/lobby';

export const LOBBY_TESTCASES: Lobby[] = [
  {
    id: 2,
    name: 'Waiting for Players',
    maxPlayers: 4,
    players: [
      {
        userId: 'u1',
        name: 'Alice',
        selectedColour: 'Red',
        isOwner: true,
        isReady: true,
        isMe: false,
      },
    ],
  },
  {
    id: 3,
    name: 'Duo Ready Up',
    maxPlayers: 2,
    players: [
      {
        userId: 'u2',
        name: 'Bob',
        selectedColour: 'Green',
        isOwner: true,
        isReady: true,
        isMe: false,
      },
      {
        userId: 'u3',
        name: 'Charlie',
        selectedColour: 'Red',
        isOwner: false,
        isReady: true,
        isMe: false,
      },
    ],
  },
  {
    id: 4,
    name: 'Almost Full',
    maxPlayers: 4,
    players: [
      {
        userId: 'u4',
        name: 'Dana',
        selectedColour: 'Red',
        isOwner: true,
        isReady: true,
        isMe: false,
      },
      {
        userId: 'u5',
        name: 'Eve',
        selectedColour: 'Green',
        isOwner: false,
        isReady: false,
        isMe: false,
      },
      {
        userId: 'u6',
        name: 'Frank',
        selectedColour: 'Yellow',
        isOwner: false,
        isReady: true,
        isMe: false,
      },
    ],
  },
  {
    id: 5,
    name: 'Full House',
    maxPlayers: 4,
    players: [
      {
        userId: 'u7',
        name: 'Grace',
        selectedColour: 'Red',
        isOwner: true,
        isReady: true,
        isMe: false,
      },
      {
        userId: 'u8',
        name: 'Heidi',
        selectedColour: 'Blue',
        isOwner: false,
        isReady: true,
        isMe: false,
      },
      {
        userId: 'u9',
        name: 'Ivan',
        selectedColour: 'Black',
        isOwner: false,
        isReady: true,
        isMe: false,
      },
      {
        userId: 'u10',
        name: 'Judy',
        selectedColour: 'Green',
        isOwner: false,
        isReady: false,
        isMe: false,
      },
    ],
  },
  {
    id: 6,
    name: 'Trio Not Ready',
    maxPlayers: 3,
    players: [
      {
        userId: 'u11',
        name: 'Karl',
        selectedColour: 'Red',
        isOwner: true,
        isReady: false,
        isMe: false,
      },
      {
        userId: 'u12',
        name: 'Liam',
        selectedColour: 'Blue',
        isOwner: false,
        isReady: false,
        isMe: false,
      },
    ],
  },
];

export const DEFAULT_TEST_LOBBY = LOBBY_TESTCASES[0];
