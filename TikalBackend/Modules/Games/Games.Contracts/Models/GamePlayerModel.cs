using Shared.Contracts.Enums;

namespace Games.Contracts.Models;

public sealed record GamePlayerModel
{
    public required string UserId { get; set; }

    public required string Name { get; set; }

    public required ColourModel Colour { get; set; }

    public required int Points { get; set; }
}