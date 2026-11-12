import { HexCoordinate, TravelCost } from '../models/tile';

// distance from the centre of a tile to its corners, in world units
export const HEX_SIZE = 1;

export type WorldPosition = {
  x: number;
  z: number;
};

/**
 * Centre of a tile on the ground plane.
 * Flat-topped hexagons (edges facing north and south) in the backend's axial coordinates (q, r):
 * north (r - 1) is -z, away from the default camera, east (q + 1) is +x.
 */
export function hexToWorld({ q, r }: HexCoordinate): WorldPosition {
  return {
    x: HEX_SIZE * 1.5 * q,
    z: HEX_SIZE * Math.sqrt(3) * (r + q / 2),
  };
}

// the neighbour across each edge of a tile, the same offsets as the backend's HexCoordinate
export const EDGE_OFFSETS: Readonly<Record<keyof TravelCost, HexCoordinate>> = {
  north: { q: 0, r: -1 },
  northEast: { q: 1, r: -1 },
  southEast: { q: 1, r: 0 },
  south: { q: 0, r: 1 },
  southWest: { q: -1, r: 1 },
  northWest: { q: -1, r: 0 },
};

// identifies a tile on the board, e.g. as a map key
export function coordinateKey({ q, r }: HexCoordinate): string {
  return `${q},${r}`;
}
