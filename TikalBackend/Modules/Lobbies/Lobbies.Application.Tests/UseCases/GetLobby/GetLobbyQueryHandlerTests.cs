using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.GetLobby;
using Lobbies.Contracts.Queries;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using MediatR;
using Moq;

namespace Lobbies.Application.Tests.UseCases.GetLobby;

public sealed class GetLobbyQueryHandlerTests
{
    // dependencies
    private readonly Mock<LobbyQueryContext> lobbyQueryContext;
    private readonly Mock<ISender> sender;

    // under test
    private readonly GetLobbyQueryHandler handler;

    public GetLobbyQueryHandlerTests()
    {
        lobbyQueryContext = new Mock<LobbyQueryContext>();
        sender = new Mock<ISender>();

        handler = new GetLobbyQueryHandler(lobbyQueryContext.Object, sender.Object);
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

        sender.Setup(s => s.Send(
            It.Is<GetAccountsQuery>(q => q.UserIds.SetEquals(lobby.Players.Select(p => p.UserId))),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((GetAccountsQuery accountsQuery, CancellationToken _) => [.. accountsQuery.UserIds.Select(id =>
            new AccountModel
            {
                Name = "Test",
                UserId = id
            })]);

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