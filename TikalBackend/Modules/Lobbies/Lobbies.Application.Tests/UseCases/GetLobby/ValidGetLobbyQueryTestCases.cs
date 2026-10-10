using Lobbies.Contracts.Queries;

namespace Lobbies.Application.Tests.UseCases.GetLobby;

internal class ValidGetLobbyQueries : TheoryData<GetLobbyQuery>
{
    public ValidGetLobbyQueries()
    {
        Add(new GetLobbyQuery(0));
        Add(new GetLobbyQuery(1));
        Add(new GetLobbyQuery(234));
        Add(new GetLobbyQuery(2349827349));
    }
}