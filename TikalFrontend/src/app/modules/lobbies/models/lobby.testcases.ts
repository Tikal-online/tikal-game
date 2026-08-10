import { Lobby } from './lobby';

export const DEFAULT_TEST_LOBBY: Lobby = {
  id: 1,
  name: 'TestLobby',
  maxPlayers: 4,
  players: [{ userId: 'u1', name: 'Alice', isOwner: true, isReady: true, isMe: false }],
};

export const LOBBY_TESTCASES: Lobby[] = [
  {
    id: 2,
    name: 'Waiting for Players',
    maxPlayers: 4,
    players: [{ userId: 'u1', name: 'Alice', isOwner: true, isReady: true, isMe: false }],
  },
  {
    id: 3,
    name: 'Duo Ready Up',
    maxPlayers: 2,
    players: [
      { userId: 'u2', name: 'Bob', isOwner: true, isReady: true, isMe: false },
      { userId: 'u3', name: 'Charlie', isOwner: false, isReady: true, isMe: false },
    ],
  },
  {
    id: 4,
    name: 'Almost Full',
    maxPlayers: 4,
    players: [
      { userId: 'u4', name: 'Dana', isOwner: true, isReady: true, isMe: false },
      { userId: 'u5', name: 'Eve', isOwner: false, isReady: false, isMe: false },
      { userId: 'u6', name: 'Frank', isOwner: false, isReady: true, isMe: false },
    ],
  },
  {
    id: 5,
    name: 'Full House',
    maxPlayers: 4,
    players: [
      { userId: 'u7', name: 'Grace', isOwner: true, isReady: true, isMe: false },
      { userId: 'u8', name: 'Heidi', isOwner: false, isReady: true, isMe: false },
      { userId: 'u9', name: 'Ivan', isOwner: false, isReady: true, isMe: false },
      { userId: 'u10', name: 'Judy', isOwner: false, isReady: false, isMe: false },
    ],
  },
  {
    id: 6,
    name: 'Trio Not Ready',
    maxPlayers: 3,
    players: [
      { userId: 'u11', name: 'Karl', isOwner: true, isReady: false, isMe: false },
      { userId: 'u12', name: 'Liam', isOwner: false, isReady: false, isMe: false },
    ],
  },
];
