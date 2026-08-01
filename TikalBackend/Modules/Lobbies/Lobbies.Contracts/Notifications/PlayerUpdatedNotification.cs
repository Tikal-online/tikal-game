using Lobbies.Contracts.Models;
using MediatR;

namespace Lobbies.Contracts.Notifications;

public sealed record PlayerUpdatedNotification(LobbyPlayerModel LobbyPlayerModel, long LobbyId) : INotification;