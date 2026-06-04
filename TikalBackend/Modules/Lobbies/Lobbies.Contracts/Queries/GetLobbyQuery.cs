using Lobbies.Contracts.Models;
using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Queries;

public sealed record GetLobbyQuery(long Id) : Query<LobbyModel?>;