export type HexCoordinate = {
  q: number;
  r: number;
};

export type TravelCost = {
  north: number;
  northEast: number;
  southEast: number;
  south: number;
  southWest: number;
  northWest: number;
};

type TileBase = {
  costs: TravelCost;
  coordinate: HexCoordinate;
};

export type EmptyTile = TileBase & {
  type: 'empty';
};

export type TreasureTile = TileBase & {
  type: 'treasure';
};

export type VolcanoTile = TileBase & {
  type: 'volcano';
};

export type TempleTile = TileBase & {
  type: 'temple';
  templeLevel: number;
};

export type Tile = EmptyTile | TreasureTile | VolcanoTile | TempleTile;

// the edges of a tile in clockwise order, starting at the top
const CLOCKWISE_EDGES: readonly (keyof TravelCost)[] = [
  'north',
  'northEast',
  'southEast',
  'south',
  'southWest',
  'northWest',
];

export function rotateCosts(costs: TravelCost, steps: number): TravelCost {
  const rotated = { ...costs };

  CLOCKWISE_EDGES.forEach((edge, index) => {
    const target = CLOCKWISE_EDGES[(((index + steps) % 6) + 6) % 6];
    rotated[target] = costs[edge];
  });

  return rotated;
}
