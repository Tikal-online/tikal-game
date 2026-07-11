using Lobbies.Domain.Entities;
using MediatR;

namespace Lobbies.Domain.Events;

public sealed record PlayerJoinedEvent(Player Player) : INotification;