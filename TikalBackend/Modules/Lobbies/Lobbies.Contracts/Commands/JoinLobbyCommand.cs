namespace Lobbies.Contracts.Commands;

public sealed record JoinLobbyCommand(string UserId, string Username, long LobbyId);