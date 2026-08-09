using Lobbies.Domain.Enums;
using Lobbies.Domain.Events;
using Shared.Domain.Entities;

namespace Lobbies.Domain.Entities;

public sealed class Player : Entity
{
    public long Id { get; set; }

    public required string UserId { get; set; }

    public required Colour SelectedColour { get; set; }

    public bool IsReady { get; set; }

    public bool IsOwner { get; set; }

    public long LobbyId { get; set; }

    public Lobby Lobby { get; set; } = null!;

    public void ReadyUp()
    {
        if (IsReady)
        {
            return;
        }

        IsReady = true;
        AddDomainEvent(new PlayerUpdatedEvent(this));
    }
}