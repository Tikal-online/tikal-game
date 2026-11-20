import { Game } from '../../models/game';
import { TravelCost } from '../../models/tile';

const COSTS: TravelCost = {
  north: 1,
  northEast: 2,
  southEast: 0,
  south: 1,
  southWest: 3,
  northWest: 2,
};

export const GAME_TESTCASES: Game[] = [
  {
    id: 1,
    players: [
      {
        userId: 'u1',
        name: 'Alice',
        colour: 'Red',
        points: 10,
        turnPosition: 0,
      },
    ],
    tiles: [
      { type: 'empty', costs: COSTS, coordinate: { q: 0, r: 0 } },
      { type: 'treasure', costs: COSTS, coordinate: { q: 1, r: 0 } },
      { type: 'volcano', costs: COSTS, coordinate: { q: 0, r: 1 } },
      { type: 'temple', costs: COSTS, coordinate: { q: -1, r: 1 }, templeLevel: 3 },
    ],
    currentTurnPosition: 0,
  },
  {
    id: 2,
    players: [
      {
        userId: 'u2',
        name: 'Bob',
        colour: 'Green',
        points: 50,
        turnPosition: 0,
      },
      {
        userId: 'u3',
        name: 'Charlie',
        colour: 'Red',
        points: 50,
        turnPosition: 1,
      },
    ],
    tiles: [],
    currentTurnPosition: 1,
  },
  {
    id: 3,
    players: [
      {
        userId: 'u4',
        name: 'Dana',
        colour: 'Blue',
        points: 5,
        turnPosition: 0,
      },
      {
        userId: 'u5',
        name: 'Eve',
        colour: 'Yellow',
        points: 8,
        turnPosition: 2,
      },
    ],
    tiles: [],
    currentTurnPosition: 2,
  },
];

export const DEFAULT_TEST_GAME = GAME_TESTCASES[0];
