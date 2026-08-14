using Lobbies.Domain.Events;
using Shared.Domain.Entities;
using Shared.Domain.Enums;

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

    public void ReadyDown()
    {
        if (!IsReady)
        {
            return;
        }

        IsReady = false;
        AddDomainEvent(new PlayerUpdatedEvent(this));
    }
}