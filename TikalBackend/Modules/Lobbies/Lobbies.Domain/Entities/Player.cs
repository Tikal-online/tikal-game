using Lobbies.Domain.Enums;

namespace Lobbies.Domain.Entities;

public sealed class Player
{
    public long Id { get; set; }

    public required string UserId { get; set; }

    public required Colour SelectedColour { get; set; }

    public bool IsReady { get; set; }

    public bool IsOwner { get; set; }

    public required long LobbyId { get; set; }

    public required Lobby Lobby { get; set; }
}