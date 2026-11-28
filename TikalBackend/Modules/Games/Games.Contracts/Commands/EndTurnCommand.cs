namespace Games.Contracts.Commands;

public sealed record EndTurnCommand(string UserId, long GameId);