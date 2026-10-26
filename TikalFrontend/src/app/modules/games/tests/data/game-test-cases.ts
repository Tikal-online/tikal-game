import { Game } from '../../models/game';

export const GAME_TESTCASES: Game[] = [
  {
    id: 1,
    players: [
      {
        userId: 'u1',
        name: 'Alice',
        colour: 'red',
        points: 10,
      },
    ],
  },
  {
    id: 2,
    players: [
      {
        userId: 'u2',
        name: 'Bob',
        colour: 'green',
        points: 50,
      },
      {
        userId: 'u3',
        name: 'Charlie',
        colour: 'red',
        points: 50,
      },
    ],
  },
];

export const DEFAULT_TEST_GAME = GAME_TESTCASES[0];
