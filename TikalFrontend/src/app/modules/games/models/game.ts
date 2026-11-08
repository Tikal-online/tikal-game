import { Player } from './player';
import { Tile } from './tile';

export type Game = {
  id: number;
  players: Player[];
  tiles: Tile[];
};
