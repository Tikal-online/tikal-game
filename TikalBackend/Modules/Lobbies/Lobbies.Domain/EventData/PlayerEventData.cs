using Shared.Domain.Enums;

namespace Lobbies.Domain.EventData;

public sealed record PlayerEventData
{
    public required string UserId { get; set; }

    public required string Name { get; set; }

    public required Colour SelectedColour { get; set; }

    public required bool IsReady { get; set; }

    public required bool IsOwner { get; set; }
}