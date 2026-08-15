using OneOf;
using OneOf.Types;
using Shared.Contracts.Enums;
using Shared.Contracts.Messaging;

namespace Games.Contracts.Commands;

public sealed record CreateGameCommand(long LobbyId, IReadOnlyDictionary<ColourModel, string> Players)
    : Command<OneOf<Success>>;