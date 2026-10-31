using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Notifications;

public sealed record LobbyStartedNotification(LobbyModel Lobby);