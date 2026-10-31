using Lobbies.Domain.EventData;
using Shared.Domain.Entities;

namespace Lobbies.Domain.Events;

public sealed record LobbyStartedEvent(LobbyEventData Lobby) : IDomainEvent;