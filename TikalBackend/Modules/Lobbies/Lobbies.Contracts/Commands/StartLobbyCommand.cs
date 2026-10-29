namespace Lobbies.Contracts.Commands;

public sealed record StartLobbyCommand(string UserId, long LobbyId);