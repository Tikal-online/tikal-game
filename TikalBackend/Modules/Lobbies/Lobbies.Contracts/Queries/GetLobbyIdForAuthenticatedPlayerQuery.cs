using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Queries;

public record GetLobbyIdForAuthenticatedPlayerQuery : Query<long?>;