import { Player } from './player';
import { Tile } from './tile';

export type Game = {
  id: number;
  players: Player[];
  tiles: Tile[];
  currentTurnPosition: number;
};

export function markAuthenticatedPlayer(game: Game, userId: string | undefined): Game {
  return {
    ...game,
    players: game.players.map((player) => ({
      ...player,
      isMe: player.userId === userId,
    })),
  };
}
