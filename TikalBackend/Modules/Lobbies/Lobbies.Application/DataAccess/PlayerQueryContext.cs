namespace Lobbies.Application.DataAccess;

public interface PlayerQueryContext
{
    Task<bool> PlayerExists(string UserId, CancellationToken cancellationToken);
}