using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.GetLobbyForAuthenticatedPlayer;
using Lobbies.Contracts.Queries;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using MediatR;
using Moq;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.GetLobbyForAuthenticatedPlayer;

public sealed class GetLobbyForAuthenticatedPlayerQueryHandlerTests
{
    // dependencies
    private readonly Mock<LobbyQueryContext> lobbyQueryContext;
    private readonly Mock<ISender> sender;
    private readonly AccountContext accountContext;

    // under test
    private readonly GetLobbyForAuthenticatedPlayerQueryHandler handler;

    public GetLobbyForAuthenticatedPlayerQueryHandlerTests()
    {
        lobbyQueryContext = new Mock<LobbyQueryContext>();
        sender = new Mock<ISender>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new GetLobbyForAuthenticatedPlayerQueryHandler(
            lobbyQueryContext.Object,
            sender.Object,
            accountContext
        );
    }

    [Fact]
    public async Task GivenAuthenticatedPlayerNotInLobby_WhenHandle_ThenReturnsNull()
    {
        // given
        lobbyQueryContext.Setup(q => q.GetByUserId(accountContext.Account.UserId))
            .ReturnsAsync(default(Lobby));

        var query = new GetLobbyForAuthenticatedPlayerQuery();

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public async Task GivenAuthenticatedPlayerInLobby_WhenHandle_ThenReturnsLobbyModel(Lobby lobby)
    {
        // given
        lobbyQueryContext.Setup(q => q.GetByUserId(accountContext.Account.UserId))
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

        var query = new GetLobbyForAuthenticatedPlayerQuery();

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