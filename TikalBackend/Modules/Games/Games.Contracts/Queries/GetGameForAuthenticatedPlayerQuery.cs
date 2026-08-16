using Games.Contracts.Models;
using Shared.Contracts.Messaging;

namespace Games.Contracts.Queries;

public sealed record GetGameForAuthenticatedPlayerQuery : Query<GameModel?>;