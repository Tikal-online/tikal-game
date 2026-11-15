using Games.Application.DataAccess;
using Games.Application.UseCases.GetNextTile;
using Games.Contracts.Errors;
using Games.Contracts.Models;
using Games.Contracts.Queries;
using Games.Domain.Entities;
using Games.Domain.Tests.Data;
using Moq;

namespace Games.Application.Tests.UseCases.GetNextTile;

public sealed class GetNextTileQueryHandlerTests
{
    // dependencies
    private readonly Mock<GameRepository> gameRepository;

    // under test
    private readonly GetNextTileQueryHandler handler;

    public GetNextTileQueryHandlerTests()
    {
        gameRepository = new Mock<GameRepository>();

        handler = new GetNextTileQueryHandler(gameRepository.Object);
    }

    private void SetupHappyPath(Game game)
    {
        // game exists
        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);
    }

    [Fact]
    public async Task GivenGameDoesntExist_WhenHandle_ThenReturnsGameNotFoundError()
    {
        // given
        var query = new GetNextTileQuery(1);

        gameRepository.Setup(r => r.GetById(1, TestContext.Current.CancellationToken))
            .ReturnsAsync(default(Game));

        // when
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<GameNotFound>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public async Task GivenGameWithTilesLeft_WhenHandle_ThenReturnsTileModel(Game game)
    {
        // given
        var query = new GetNextTileQuery(game.Id);

        SetupHappyPath(game);

        // when
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<TileModel>(result.Value, exactMatch: false);
    }
}