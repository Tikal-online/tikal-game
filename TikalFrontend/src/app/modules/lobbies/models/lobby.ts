import { Player } from './player';

export type Lobby = {
  id: number;
  name: string;
  maxPlayers: number;
  players: Player[];
};

export function markAuthenticatedPlayer(lobby: Lobby, userId: string | undefined): Lobby {
  return {
    ...lobby,
    players: lobby.players.map((player) => ({
      ...player,
      isMe: player.userId === userId,
    })),
  };
}
