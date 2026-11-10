import { Tile, TravelCost } from '../models/tile';

// placeholder until the backend sends the whole board: a rectangle of jungle tiles
export const PLACEHOLDER_COLUMNS = 8;

export const PLACEHOLDER_ROWS = 5;

const NO_COSTS: TravelCost = {
  north: 0,
  northEast: 0,
  southEast: 0,
  south: 0,
  southWest: 0,
  northWest: 0,
};

/**
 * Columns follow q, every other column sits half a tile further south (see hexToWorld),
 * so r starts lower in each second column to keep the rows of the rectangle straight.
 */
export function placeholderTiles(): Tile[] {
  const tiles: Tile[] = [];

  for (let column = 0; column < PLACEHOLDER_COLUMNS; column++) {
    for (let row = 0; row < PLACEHOLDER_ROWS; row++) {
      tiles.push({
        type: 'empty',
        costs: NO_COSTS,
        coordinate: { q: column, r: row - Math.floor(column / 2) },
      });
    }
  }

  return tiles;
}
