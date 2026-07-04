import { Player } from './player';

export type Lobby = {
  id: number;
  name: string;
  maxPlayers: number;
  players: Player[];
};
