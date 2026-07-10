using Lobbies.Contracts.Models;
using MediatR;

namespace Lobbies.Contracts.Notifications;

public sealed record PlayerJoinedNotification(LobbyPlayerModel lobbyPlayerModel, long LobbyId) : INotification;