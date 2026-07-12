using Lobbies.Contracts.Models;
using MediatR;

namespace Lobbies.Contracts.Notifications;

public sealed record LobbyChatMessageSentNotification(ChatMessageModel ChatMessageModel, long LobbyId) : INotification;