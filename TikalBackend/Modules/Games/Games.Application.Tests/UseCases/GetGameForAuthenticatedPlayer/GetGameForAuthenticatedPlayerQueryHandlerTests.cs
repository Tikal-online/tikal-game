using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Games.Application.DataAccess;
using Games.Application.UseCases.GetGameForAuthenticatedPlayer;
using Games.Contracts.Queries;
using Games.Domain.Entities;
using Games.Domain.Tests.Data;
using Moq;
using Shared.Application.Contexts;
using Shared.Application.Tests;
using Shared.Contracts.Enums;

namespace Games.Application.Tests.UseCases.GetGameForAuthenticatedPlayer;

public sealed class GetGameForAuthenticatedPlayerQueryHandlerTests
{
    // dependencies
    private readonly Mock<GameQueryContext> gameQueryContext;
    private readonly AccountContext accountContext;

    // under test
    private readonly GetGameForAuthenticatedPlayerQueryHandler handler;

    public GetGameForAuthenticatedPlayerQueryHandlerTests()
    {
        gameQueryContext = new Mock<GameQueryContext>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new GetGameForAuthenticatedPlayerQueryHandler(
            gameQueryContext.Object,
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
                        () => Assert.Equal(game.Players.ElementAt(i).Name, result?.Players.ElementAt(i).Name),
                        () => Assert.Equal((ColourModel)game.Players.ElementAt(i).Colour, result?.Players.ElementAt(i).Colour)
                    );
                }
            }
        );
    }
}