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
