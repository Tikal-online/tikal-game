using Games.Domain.EventData;
using Shared.Domain.Entities;

namespace Games.Domain.Events;

public sealed record PlayerLeftDomainEvent(PlayerEventData Player, long LobbyId) : IDomainEvent;