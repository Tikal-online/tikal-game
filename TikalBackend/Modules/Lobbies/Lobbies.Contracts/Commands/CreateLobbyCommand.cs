namespace Lobbies.Contracts.Commands;

public sealed record CreateLobbyCommand(string Name, int MaxPlayers);