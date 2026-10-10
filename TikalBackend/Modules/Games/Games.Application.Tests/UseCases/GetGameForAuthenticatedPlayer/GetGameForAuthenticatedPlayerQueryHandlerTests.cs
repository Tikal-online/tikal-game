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

public sealed class GetGameForAuthenticatedPlayerQueryHandlerTests
{
    // dependencies
    private readonly Mock<GameQueryContext> gameQueryContext;
    private readonly Mock<ISender> sender;
    private readonly AccountContext accountContext;

    // under test
    private readonly GetGameForAuthenticatedPlayerQueryHandler handler;

    public GetGameForAuthenticatedPlayerQueryHandlerTests()
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

    [Fact]
    public async Task GivenAuthenticatedPlayerNotInGame_WhenHandle_ThenReturnsNull()
    {
        // given
        gameQueryContext.Setup(q => q.GetByUserId(accountContext.Account.UserId))
            .ReturnsAsync(default(Game));

        var query = new GetGameForAuthenticatedPlayerQuery();

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public async Task GivenAuthenticatedPlayerInGame_WhenHandle_ThenReturnsGameModel(Game game)
    {
        // given
        gameQueryContext.Setup(q => q.GetByUserId(accountContext.Account.UserId))
            .ReturnsAsync(game);

        sender.Setup(s => s.Send(
            It.Is<GetAccountsQuery>(q => q.UserIds.SetEquals(game.Players.Select(p => p.UserId))),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync((GetAccountsQuery accountsQuery, CancellationToken _) => [.. accountsQuery.UserIds.Select(id =>
            new AccountModel
            {
                Name = "Test",
                UserId = id
            })]);

        var query = new GetGameForAuthenticatedPlayerQuery();

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(game.Id, result?.Id),
            () => Assert.Equal(game.Players.Count, result?.Players.Count),
            () =>
            {
                for (var i = 0; i < game.Players.Count; i++)
                {
                    Assert.Multiple(
                        () => Assert.Equal(game.Players.ElementAt(i).UserId, result?.Players.ElementAt(i).UserId),
                        () => Assert.Equal((ColourModel)game.Players.ElementAt(i).Colour, result?.Players.ElementAt(i).Colour)
                    );
                }
            }
        );
    }
}