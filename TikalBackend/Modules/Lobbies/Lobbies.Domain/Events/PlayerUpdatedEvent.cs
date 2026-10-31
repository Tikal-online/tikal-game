using Lobbies.Domain.EventData;
using Shared.Domain.Entities;

namespace Lobbies.Domain.Events;

public sealed record PlayerUpdatedEvent(PlayerEventData Player, long LobbyId) : IDomainEvent;