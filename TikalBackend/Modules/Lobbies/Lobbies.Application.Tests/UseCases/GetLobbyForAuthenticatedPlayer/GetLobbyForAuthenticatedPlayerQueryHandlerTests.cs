using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.GetLobbyForAuthenticatedPlayer;
using Lobbies.Contracts.Enums;
using Lobbies.Contracts.Queries;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using MediatR;
using Moq;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.GetLobbyForAuthenticatedPlayer;

internal sealed class GetLobbyForAuthenticatedPlayerQueryHandlerTests
{
    // dependencies
    private Mock<LobbyQueryContext> lobbyQueryContext;
    private Mock<ISender> sender;
    private AccountContext accountContext;

    // under test
    private GetLobbyForAuthenticatedPlayerQueryHandler handler;

    [SetUp]
    public void Setup()
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

    [Test]
    public async Task GivenAuthenticatedPlayerNotInLobby_WhenHandle_ThenReturnsNull()
    {
        // given
        lobbyQueryContext.Setup(q => q.GetByUserIdAsync(accountContext.Account.UserId))
            .ReturnsAsync(default(Lobby));

        var query = new GetLobbyForAuthenticatedPlayerQuery();

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.That(result, Is.Null);
    }

    [TestCaseSource(typeof(LobbyTestCases), nameof(LobbyTestCases.ValidLobbyTestCases))]
    public async Task GivenAuthenticatedPlayerInLobby_WhenHandle_ThenReturnsLobbyModel(Lobby lobby)
    {
        // given
        lobbyQueryContext.Setup(q => q.GetByUserIdAsync(accountContext.Account.UserId))
            .ReturnsAsync(lobby);

        sender.Setup(s => s.Send(
            It.Is<GetAccountsQuery>(q => q.UserIds.SetEquals(lobby.Players.Select(p => p.UserId))),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((GetAccountsQuery accountsQuery, CancellationToken _) => accountsQuery.UserIds.Select(id =>
            new AccountModel
            {
                Name = "Test",
                UserId = id
            }).ToList());

        var query = new GetLobbyForAuthenticatedPlayerQuery();

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(lobby.Id));
            Assert.That(result.Name, Is.EqualTo(lobby.Name));
            Assert.That(result.MaxPlayers, Is.EqualTo(lobby.MaxPlayers));
            Assert.That(result.Players, Has.Count.EqualTo(lobby.Players.Count));

            for (var i = 0; i < lobby.Players.Count; i++)
            {
                Assert.That(result.Players[i].UserId, Is.EqualTo(lobby.Players.ElementAt(i).UserId));
                Assert.That(result.Players[i].IsOwner, Is.EqualTo(lobby.Players.ElementAt(i).IsOwner));
                Assert.That(result.Players[i].IsReady, Is.EqualTo(lobby.Players.ElementAt(i).IsReady));
                Assert.That(
                    result.Players[i].SelectedColour,
                    Is.EqualTo((ColourModel)lobby.Players.ElementAt(i).SelectedColour)
                );
            }
        }
    }
}