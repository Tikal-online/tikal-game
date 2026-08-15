using Shared.Domain.Enums;

namespace Games.Domain.Entities;

public sealed class Player
{
    public long Id { get; set; }

    public required string UserId { get; set; }

    public required Colour Colour { get; set; }

    public required int Points { get; set; }

    public long GameId { get; set; }

    public Game Game { get; set; } = null!;

    public ICollection<TroopAssignment> TroopAssignments { get; set; } = [];
}