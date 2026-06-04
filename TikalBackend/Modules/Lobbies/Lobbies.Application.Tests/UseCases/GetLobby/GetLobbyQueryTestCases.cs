using Lobbies.Contracts.Queries;

namespace Lobbies.Application.Tests.UseCases.GetLobby;

internal static class GetLobbyQueryTestCases
{
    public static IEnumerable<GetLobbyQuery> ValidGetLobbyQueries =>
    [
        new(0),
        new(1),
        new(234),
        new(2349827349)
    ];
}