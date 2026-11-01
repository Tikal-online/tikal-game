using Shared.Domain.Enums;

namespace Games.Domain.EventData;

public sealed record PlayerEventData
{
    public required string UserId { get; set; }

    public required string Name { get; set; }

    public required Colour Colour { get; set; }

    public required int Points { get; set; }
}