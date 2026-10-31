using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Notifications;

public sealed record PlayerJoinedNotification(LobbyPlayerModel LobbyPlayerModel, long LobbyId);