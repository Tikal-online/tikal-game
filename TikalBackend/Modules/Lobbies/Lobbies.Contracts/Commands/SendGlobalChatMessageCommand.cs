using OneOf.Types;
using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Commands;

public sealed record SendGlobalChatMessageCommand(string messageContent) : Command<Success>;