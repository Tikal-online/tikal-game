using Lobbies.Domain.Entities;
using Shared.Domain.Entities;

namespace Lobbies.Domain.Events;

public sealed record PlayerUpdatedEvent(Player Player) : IDomainEvent;