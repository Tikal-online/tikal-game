using Lobbies.Domain.Entities;
using Shared.Domain.Entities;

namespace Lobbies.Domain.Events;

public sealed record LobbyStartedEvent(Lobby lobby) : IDomainEvent;