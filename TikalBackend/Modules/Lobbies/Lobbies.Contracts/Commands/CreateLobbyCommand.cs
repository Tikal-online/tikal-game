namespace Lobbies.Contracts.Commands;

public sealed record CreateLobbyCommand(string UserId, string Username, string Name, int MaxPlayers);