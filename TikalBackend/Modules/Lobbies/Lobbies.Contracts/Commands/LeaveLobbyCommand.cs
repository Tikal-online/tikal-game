namespace Lobbies.Contracts.Commands;

public sealed record LeaveLobbyCommand(string UserId, long LobbyId);