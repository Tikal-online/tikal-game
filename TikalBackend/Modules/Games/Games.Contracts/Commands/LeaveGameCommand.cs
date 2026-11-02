namespace Games.Contracts.Commands;

public sealed record LeaveGameCommand(string UserId, long GameId);