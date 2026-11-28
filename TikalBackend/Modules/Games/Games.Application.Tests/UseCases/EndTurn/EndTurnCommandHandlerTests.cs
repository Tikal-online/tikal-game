using Games.Application.DataAccess;
using Games.Application.UseCases.EndTurn;
using Games.Contracts.Commands;
using Games.Contracts.Errors;
using Games.Domain.Entities;
using Games.Domain.Tests.Data;
using Moq;
using OneOf.Types;
using Shared.Contracts.Errors;

namespace Games.Application.Tests.UseCases.EndTurn;

public sealed class EndTurnCommandHandlerTests
{
    // dependencies
    private readonly Mock<GameRepository> gameRepository;

    // under test
    private readonly EndTurnCommandHandler handler;

    public EndTurnCommandHandlerTests()
    {
        gameRepository = new Mock<GameRepository>();

        handler = new EndTurnCommandHandler(gameRepository.Object);
    }

    private void SetupHappyPath(Game game, EndTurnCommand command)
    {
        // game exists
        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        // game contains authenticated player
        var player = game.Players.First();
        player.UserId = command.UserId;

        // authenticated player is current player
        game.CurrentTurnPosition = player.TurnPosition;
    }

    [Fact]
    public async Task GivenGameDoesntExist_WhenHandle_ThenReturnsGameNotFoundError()
    {
        // given
        var command = new EndTurnCommand("userId", 1);

        gameRepository.Setup(r => r.GetById(1, TestContext.Current.CancellationToken))
            .ReturnsAsync(default(Game));

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<GameNotFound>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public async Task GivenPlayerIsNotPartOfGame_WhenHandle_ThenReturnsPlayerNotInGivenGameError(Game game)
    {
        // given
        var command = new EndTurnCommand("userId", game.Id);

        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<Unprivileged>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public async Task GivenPlayerIsNotCurrentPlayer_WhenHandle_ThenReturnsPlayerNotInGivenGameError(Game game)
    {
        // given
        var command = new EndTurnCommand("userId", game.Id);

        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        var player = game.Players.First();
        player.UserId = command.UserId;

        var currentPlayer = game.Players.First(p => p.UserId != "userId");
        game.CurrentTurnPosition = currentPlayer.TurnPosition;

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<Unprivileged>(result.Value);
    }

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public async Task GivenGameInPlaceTilePhase_WhenHandle_ThenReturnsInvalidTurnPhase(Game game)
    {
        // given
        var command = new EndTurnCommand("userId", game.Id);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<InvalidTurnPhase>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ActionPhaseGames))]
    public async Task GivenGameInActionPhase_WhenHandle_ThenReturnsSuccess(Game game)
    {
        // given
        var command = new EndTurnCommand("userId", game.Id);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<Success>(result.Value);
    }
}