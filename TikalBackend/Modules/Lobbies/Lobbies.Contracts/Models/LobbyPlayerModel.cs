using Lobbies.Contracts.Enums;

namespace Lobbies.Contracts.Models;

public sealed record LobbyPlayerModel
{
    public required string UserId { get; set; }

    public required string Name { get; set; }

    public required ColourModel SelectedColour { get; set; }

    public required bool IsReady { get; set; }

    public required bool IsOwner { get; set; }
}