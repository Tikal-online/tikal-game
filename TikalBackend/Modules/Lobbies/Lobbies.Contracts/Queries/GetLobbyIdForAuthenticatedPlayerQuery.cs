using Shared.Contracts.Messaging;

namespace Lobbies.Contracts.Queries;

public sealed record GetLobbyIdForAuthenticatedPlayerQuery : Query<long?>;