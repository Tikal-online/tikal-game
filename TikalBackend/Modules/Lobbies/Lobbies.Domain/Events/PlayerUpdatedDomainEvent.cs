using Lobbies.Domain.EventData;
using Shared.Domain.Entities;

namespace Lobbies.Domain.Events;

public sealed record PlayerUpdatedDomainEvent(PlayerEventData Player, long LobbyId) : IDomainEvent;