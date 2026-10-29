using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Notifications;

public sealed record PlayerLeftNotification(LobbyPlayerModel LobbyPlayerModel, long LobbyId);