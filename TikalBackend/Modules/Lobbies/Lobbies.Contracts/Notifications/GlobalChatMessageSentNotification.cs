using Lobbies.Contracts.Models;
using MediatR;

namespace Lobbies.Contracts.Notifications;

public sealed record GlobalChatMessageSentNotification(ChatMessageModel message) : INotification;