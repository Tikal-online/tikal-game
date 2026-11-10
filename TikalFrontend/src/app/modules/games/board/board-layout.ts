import { HexCoordinate } from '../models/tile';

// the board is a rectangle of hexagons, tiles are set on it as the game goes on
export const BOARD_COLUMNS = 8;

export const BOARD_ROWS = 5;

/**
 * Every place on the board a tile can be set on.
 * The start tile (0, 0) is the south west corner, q grows to the east and r to the south.
 * Every other column sits half a tile further south (see hexToWorld),
 * so r starts lower in each second column to keep the rows of the rectangle straight.
 */
export function boardCoordinates(): HexCoordinate[] {
  const coordinates: HexCoordinate[] = [];

  for (let q = 0; q < BOARD_COLUMNS; q++) {
    // r of the southernmost place in this column
    const southR = -Math.floor(q / 2);

    for (let row = 0; row < BOARD_ROWS; row++) {
      coordinates.push({ q, r: southR - row });
    }
  }

  return coordinates;
}
