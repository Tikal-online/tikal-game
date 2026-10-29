using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Notifications;

public sealed record PlayerUpdatedNotification(LobbyPlayerModel LobbyPlayerModel, long LobbyId);