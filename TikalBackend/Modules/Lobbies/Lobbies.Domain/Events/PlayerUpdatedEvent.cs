using Lobbies.Domain.Entities;
using MediatR;

namespace Lobbies.Domain.Events;

public sealed record PlayerUpdatedEvent(Player Player) : INotification;