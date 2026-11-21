namespace Games.Contracts.Commands;

public sealed record PlaceTileCommand(string UserId, long GameId, int Q, int R, int Rotation);