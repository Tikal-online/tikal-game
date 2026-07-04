using Lobbies.Contracts.Models;
using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Queries;

public record GetLobbyForAuthenticatedPlayerQuery : Query<LobbyModel?>;