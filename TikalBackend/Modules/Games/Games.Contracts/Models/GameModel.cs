using Games.Contracts.Enums;

namespace Games.Contracts.Models;

public sealed record GameModel
{
    public required long Id { get; set; }

    public required long LobbyId { get; set; }

    public List<GamePlayerModel> Players { get; set; } = [];

    public List<TileModel> Tiles { get; init; } = [];

    public int CurrentTurnPosition { get; set; }

    public TurnPhaseModel TurnPhase { get; set; }
}