import { Player } from './player';
import { Tile } from './tile';
import { TurnPhase } from './turn-phase';

export type Game = {
  id: number;
  players: Player[];
  tiles: Tile[];
  currentTurnPosition: number;
  turnPhase: TurnPhase;
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
