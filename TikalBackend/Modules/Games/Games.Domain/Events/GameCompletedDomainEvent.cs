using Games.Domain.EventData;
using Shared.Domain.Entities;

namespace Games.Domain.Events;

public sealed record GameCompletedDomainEvent(GameEventData Game) : IDomainEvent;