using OneOf.Types;
using Shared.Contracts.Messaging;

namespace Games.Contracts.Commands;

public sealed record CreateGameCommand : Command<Success>;