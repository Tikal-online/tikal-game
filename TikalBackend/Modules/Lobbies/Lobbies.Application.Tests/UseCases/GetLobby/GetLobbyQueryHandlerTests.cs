using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.Tests.Data;
using Lobbies.Application.UseCases.GetLobby;
using Lobbies.Contracts.Enums;
using Lobbies.Contracts.Queries;
using Lobbies.Domain.Entities;
using MediatR;
using Moq;

namespace Lobbies.Application.Tests.UseCases.GetLobby;

internal sealed class GetLobbyQueryHandlerTests
{
    // dependencies
    private Mock<LobbyQueryContext> lobbyQueryContext;
    private Mock<ISender> sender;

    // under test
    private GetLobbyQueryHandler handler;

    [SetUp]
    public void Setup()
    {
        lobbyQueryContext = new Mock<LobbyQueryContext>();
        sender = new Mock<ISender>();

        handler = new GetLobbyQueryHandler(lobbyQueryContext.Object, sender.Object);
    }

    [TestCaseSource(typeof(GetLobbyQueryTestCases), nameof(GetLobbyQueryTestCases.ValidGetLobbyQueries))]
    public async Task GivenNoLobbyWithId_WhenHandle_ThenReturnsNull(GetLobbyQuery query)
    {
        // given
        lobbyQueryContext.Setup(q => q.GetByIdAsync(query.Id))
            .ReturnsAsync(default(Lobby));

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.That(result, Is.Null);
    }

    [TestCaseSource(typeof(LobbyTestCases), nameof(LobbyTestCases.ValidLobbyTestCases))]
    public async Task GivenLobbyWithId_WhenHandle_ThenReturnsLobbyModel(Lobby lobby)
    {
        // given
        var query = new GetLobbyQuery(lobby.Id);

        lobbyQueryContext.Setup(q => q.GetByIdAsync(query.Id))
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

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(lobby.Id));
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