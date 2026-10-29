using Games.Application.DataAccess;
using Games.Application.UseCases.GetGameForPlayer;
using Games.Contracts.Queries;
using Games.Domain.Entities;
using Games.Domain.Tests.Data;
using Moq;
using Shared.Contracts.Enums;

namespace Games.Application.Tests.UseCases.GetGameForPlayer;

public sealed class GetGameForPlayerQueryHandlerTests
{
    // dependencies
    private readonly Mock<GameQueryContext> gameQueryContext;

    // under test
    private readonly GetGameForPlayerQueryHandler handler;

    public GetGameForPlayerQueryHandlerTests()
    {
        gameQueryContext = new Mock<GameQueryContext>();

        handler = new GetGameForPlayerQueryHandler(gameQueryContext.Object);
    }

    [Fact]
    public async Task GivenAuthenticatedPlayerNotInGame_WhenHandle_ThenReturnsNull()
    {
        // given
        var query = new GetGameForPlayerQuery("userId");

        gameQueryContext.Setup(q => q.GetByUserId(query.UserId))
            .ReturnsAsync(default(Game));

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
        var query = new GetGameForPlayerQuery("userId");

        gameQueryContext.Setup(q => q.GetByUserId(query.UserId))
            .ReturnsAsync(game);

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