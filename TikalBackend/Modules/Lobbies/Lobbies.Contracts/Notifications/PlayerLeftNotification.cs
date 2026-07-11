using Lobbies.Contracts.Models;
using MediatR;

namespace Lobbies.Contracts.Notifications;

public sealed record PlayerLeftNotification(LobbyPlayerModel LobbyPlayerModel, long LobbyId) : INotification;