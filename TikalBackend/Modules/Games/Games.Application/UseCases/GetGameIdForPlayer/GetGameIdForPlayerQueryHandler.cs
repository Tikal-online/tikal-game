using Games.Application.DataAccess;
using Games.Contracts.Queries;

namespace Games.Application.UseCases.GetGameIdForPlayer;

public sealed class GetGameIdForPlayerQueryHandler
{
    private readonly GameQueryContext gameQueryContext;

    public GetGameIdForPlayerQueryHandler(GameQueryContext gameQueryContext)
    {
        this.gameQueryContext = gameQueryContext;
    }

    public Task<long?> Handle(GetGameIdForPlayerQuery query, CancellationToken cancellationToken)
    {
        return gameQueryContext.GetIdByUserId(query.UserId, cancellationToken);
    }
}