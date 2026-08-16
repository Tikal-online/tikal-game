using Lobbies.Domain.Entities;
using MediatR;

namespace Lobbies.Domain.Events;

public sealed record LobbyStartedEvent(Lobby lobby) : INotification;