import { Lobby } from './lobby';

export const DEFAULT_TEST_LOBBY: Lobby = {
  id: 1,
  name: 'TestLobby',
  maxPlayers: 4,
  players: [{ userId: 'u1', name: 'Alice', isOwner: true, isReady: true }],
};

export const LOBBY_TESTCASES: Lobby[] = [
  {
    id: 1,
    name: 'Empty Lobby',
    maxPlayers: 4,
    players: [],
  },
  {
    id: 2,
    name: 'Waiting for Players',
    maxPlayers: 4,
    players: [{ userId: 'u1', name: 'Alice', isOwner: true, isReady: true }],
  },
  {
    id: 3,
    name: 'Duo Ready Up',
    maxPlayers: 2,
    players: [
      { userId: 'u2', name: 'Bob', isOwner: true, isReady: true },
      { userId: 'u3', name: 'Charlie', isOwner: false, isReady: true },
    ],
  },
  {
    id: 4,
    name: 'Almost Full',
    maxPlayers: 4,
    players: [
      { userId: 'u4', name: 'Dana', isOwner: true, isReady: true },
      { userId: 'u5', name: 'Eve', isOwner: false, isReady: false },
      { userId: 'u6', name: 'Frank', isOwner: false, isReady: true },
    ],
  },
  {
    id: 5,
    name: 'Full House',
    maxPlayers: 4,
    players: [
      { userId: 'u7', name: 'Grace', isOwner: true, isReady: true },
      { userId: 'u8', name: 'Heidi', isOwner: false, isReady: true },
      { userId: 'u9', name: 'Ivan', isOwner: false, isReady: true },
      { userId: 'u10', name: 'Judy', isOwner: false, isReady: false },
    ],
  },
  {
    id: 6,
    name: 'Trio Not Ready',
    maxPlayers: 3,
    players: [
      { userId: 'u11', name: 'Karl', isOwner: true, isReady: false },
      { userId: 'u12', name: 'Liam', isOwner: false, isReady: false },
    ],
  },
];
