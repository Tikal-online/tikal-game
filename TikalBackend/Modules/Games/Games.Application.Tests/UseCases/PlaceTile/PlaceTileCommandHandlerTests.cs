using Games.Application.DataAccess;
using Games.Application.UseCases.PlaceTile;
using Games.Contracts.Commands;
using Games.Contracts.Errors;
using Games.Domain.Entities;
using Games.Domain.Tests.Data;
using Games.Domain.Types;
using Moq;
using OneOf.Types;
using Shared.Contracts.Errors;

namespace Games.Application.Tests.UseCases.PlaceTile;

public sealed class PlaceTileCommandHandlerTests
{
    // dependencies
    private readonly Mock<GameRepository> gameRepository;

    // under test
    private readonly PlaceTileCommandHandler handler;

    public PlaceTileCommandHandlerTests()
    {
        gameRepository = new Mock<GameRepository>();

        handler = new PlaceTileCommandHandler(gameRepository.Object);
    }

    private void SetupHappyPath(Game game, PlaceTileCommand command)
    {
        // game exists
        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        // game contains authenticated player
        var player = game.Players.First();
        player.UserId = command.UserId;

        // player is the current player
        game.CurrentTurnPosition = player.TurnPosition;
    }

    [Fact]
    public async Task GivenGameDoesntExist_WhenHandle_ThenReturnsGameNotFoundError()
    {
        // given
        var command = new PlaceTileCommand("user-id", 1, 1, 1, 1);

        gameRepository.Setup(r => r.GetById(1, TestContext.Current.CancellationToken))
            .ReturnsAsync(default(Game));

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // when
        Assert.IsType<GameNotFound>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public async Task GivenPlayerIsNotPartOfGame_WhenHandle_ThenReturnsUnpriviligedError(Game game)
    {
        // given
        var command = new PlaceTileCommand("user-id", game.Id, 1, 1, 1);

        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // when
        Assert.IsType<Unprivileged>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public async Task GivenPlayerIsNotCurrentPlayer_WhenHandle_ThenReturnsUnpriviligedError(Game game)
    {
        // given
        var command = new PlaceTileCommand("user-id", game.Id, 1, 1, 1);

        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        var player = game.Players.First();
        player.UserId = command.UserId;

        game.CurrentTurnPosition = game.Players.First(p => p.UserId != command.UserId).TurnPosition;

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // when
        Assert.IsType<Unprivileged>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ActionPhaseGames))]
    public async Task GivenGameInActionPhase_WhenHandle_ThenReturnsInvalidTurnPhaseError(Game game)
    {
        // given
        var command = new PlaceTileCommand("user-id", game.Id, 1, 1, 1);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // when
        Assert.IsType<InvalidTurnPhase>(result.Value);
    }

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public async Task GivenGameWithoutDrawing_WhenHandle_ThenReturnsTileMustBeDrawnFirstError(Game game)
    {
        // given
        var command = new PlaceTileCommand("user-id", game.Id, 1, 1, 1);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // when
        Assert.IsType<TileMustBeDrawnFirst>(result.Value);
    }

    [Theory]
    [ClassData(typeof(InvalidTilePlacementGames))]
    public async Task GivenInvalidTilePosition_WhenHandle_ThenReturnsTileCannotBePlacedError(
        Game game,
        HexCoordinate coordinate,
        int rotation
    )
    {
        // given
        var command = new PlaceTileCommand("user-id", game.Id, coordinate.Q, coordinate.R, rotation);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // when
        Assert.IsType<TileCannotBePlaced>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidTilePlacementGames))]
    public async Task GivenValidTilePosition_WhenHandle_ThenReturnsSuccess(
        Game game,
        HexCoordinate coordinate,
        int rotation
    )
    {
        // given
        var command = new PlaceTileCommand("user-id", game.Id, coordinate.Q, coordinate.R, rotation);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // when
        Assert.IsType<Success>(result.Value);
    }
}