using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.GetLobbyForPlayer;
using Lobbies.Contracts.Queries;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;

namespace Lobbies.Application.Tests.UseCases.GetLobbyForPlayer;

public sealed class GetLobbyForPlayerQueryHandlerTests
{
    // dependencies
    private readonly Mock<LobbyQueryContext> lobbyQueryContext;

    // under test
    private readonly GetLobbyForPlayerQueryHandler handler;

    public GetLobbyForPlayerQueryHandlerTests()
    {
        lobbyQueryContext = new Mock<LobbyQueryContext>();

        handler = new GetLobbyForPlayerQueryHandler(lobbyQueryContext.Object);
    }

    [Fact]
    public async Task GivenPlayerNotInLobby_WhenHandle_ThenReturnsNull()
    {
        // given
        var query = new GetLobbyForPlayerQuery("userId");

        lobbyQueryContext.Setup(q => q.GetByUserId(query.UserId))
            .ReturnsAsync(default(Lobby));

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public async Task GivenPlayerInLobby_WhenHandle_ThenReturnsLobbyModel(Lobby lobby)
    {
        // given
        var query = new GetLobbyForPlayerQuery("userId");

        lobbyQueryContext.Setup(q => q.GetByUserId(query.UserId))
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