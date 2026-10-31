using Lobbies.Contracts.Models;

namespace Lobbies.Contracts.Notifications;

public sealed record LobbyChatMessageSentNotification(ChatMessageModel ChatMessageModel, long LobbyId);