using Lobbies.Domain.Entities;
using Shared.Domain.Entities;

namespace Lobbies.Domain.Events;

public sealed record PlayerLeftEvent(Player Player) : IDomainEvent;