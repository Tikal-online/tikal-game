using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Games.Application.DataAccess;
using Games.Application.UseCases.GetGameForAuthenticatedPlayer;
using Games.Contracts.Queries;
using Games.Domain.Entities;
using Games.Domain.Tests.Data;
using MediatR;
using Moq;
using Shared.Application.Contexts;
using Shared.Application.Tests;
using Shared.Contracts.Enums;

namespace Games.Application.Tests.UseCases.GetGameForAuthenticatedPlayer;

internal sealed class GetGameForAuthenticatedPlayerQueryHandlerTests
{
    // dependencies
    private Mock<GameQueryContext> gameQueryContext;
    private Mock<ISender> sender;
    private AccountContext accountContext;

    // under test
    private GetGameForAuthenticatedPlayerQueryHandler handler;

    [SetUp]
    public void Setup()
    {
        gameQueryContext = new Mock<GameQueryContext>();
        sender = new Mock<ISender>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new GetGameForAuthenticatedPlayerQueryHandler(
            gameQueryContext.Object,
            sender.Object,
            accountContext
        );
    }

    [Test]
    public async Task GivenAuthenticatedPlayerNotInGame_WhenHandle_ThenReturnsNull()
    {
        // given
        gameQueryContext.Setup(q => q.GetByUserId(accountContext.Account.UserId))
            .ReturnsAsync(default(Game));

        var query = new GetGameForAuthenticatedPlayerQuery();

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.That(result, Is.Null);
    }

    [TestCaseSource(typeof(GameTestCases), nameof(GameTestCases.ValidGameTestCases))]
    public async Task GivenAuthenticatedPlayerInGame_WhenHandle_ThenReturnsGameModel(Game game)
    {
        // given
        gameQueryContext.Setup(q => q.GetByUserId(accountContext.Account.UserId))
            .ReturnsAsync(game);

        sender.Setup(s => s.Send(
            It.Is<GetAccountsQuery>(q => q.UserIds.SetEquals(game.Players.Select(p => p.UserId))),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((GetAccountsQuery accountsQuery, CancellationToken _) => accountsQuery.UserIds.Select(id =>
            new AccountModel
            {
                Name = "Test",
                UserId = id
            }).ToList());

        var query = new GetGameForAuthenticatedPlayerQuery();

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(game.Id));
            Assert.That(result.Players, Has.Count.EqualTo(game.Players.Count));

            for (var i = 0; i < game.Players.Count; i++)
            {
                Assert.That(result.Players[i].UserId, Is.EqualTo(game.Players.ElementAt(i).UserId));
                Assert.That(result.Players[i].Colour, Is.EqualTo((ColourModel)game.Players.ElementAt(i).Colour));
            }
        }
    }
}