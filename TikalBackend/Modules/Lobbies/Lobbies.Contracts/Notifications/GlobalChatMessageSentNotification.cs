using MediatR;

namespace Lobbies.Contracts.Notifications;

public sealed record GlobalChatMessageSentNotification(string message) : INotification;