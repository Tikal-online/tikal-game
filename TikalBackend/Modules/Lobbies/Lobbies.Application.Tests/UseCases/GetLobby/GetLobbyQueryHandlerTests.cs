using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.GetLobby;
using Lobbies.Contracts.Queries;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;

namespace Lobbies.Application.Tests.UseCases.GetLobby;

public sealed class GetLobbyQueryHandlerTests
{
    // dependencies
    private readonly Mock<LobbyQueryContext> lobbyQueryContext;

    // under test
    private readonly GetLobbyQueryHandler handler;

    public GetLobbyQueryHandlerTests()
    {
        lobbyQueryContext = new Mock<LobbyQueryContext>();

        handler = new GetLobbyQueryHandler(lobbyQueryContext.Object);
    }

    [Theory]
    [ClassData(typeof(ValidGetLobbyQueries))]
    public async Task GivenNoLobbyWithId_WhenHandle_ThenReturnsNull(GetLobbyQuery query)
    {
        // given
        lobbyQueryContext.Setup(q => q.GetById(query.Id))
            .ReturnsAsync(default(Lobby));

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public async Task GivenLobbyWithId_WhenHandle_ThenReturnsLobbyModel(Lobby lobby)
    {
        // given
        var query = new GetLobbyQuery(lobby.Id);

        lobbyQueryContext.Setup(q => q.GetById(query.Id))
            .ReturnsAsync(lobby);

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(lobby.Id, result?.Id),
            () => Assert.Equal(lobby.Name, result?.Name),
            () => Assert.Equal(lobby.MaxPlayers, result?.MaxPlayers),
            () => Assert.Equal(lobby.Players.Count, result?.Players.Count),
            () => Assert.Equal(lobby.InGame, result?.InGame)
        );
    }
}