using Games.Application.DataAccess;
using Games.Application.UseCases.LeaveGame;
using Games.Contracts.Commands;
using Games.Contracts.Errors;
using Games.Domain.Entities;
using Games.Domain.Tests.Data;
using Moq;
using OneOf.Types;

namespace Games.Application.Tests.UseCases.LeaveGame;

public sealed class LeaveGameCommandHandlerTests
{
    // dependencies
    private readonly Mock<PlayerRepository> playerRepository;
    private readonly Mock<GameRepository> gameRepository;

    // under test
    private readonly LeaveGameCommandHandler handler;

    public LeaveGameCommandHandlerTests()
    {
        playerRepository = new Mock<PlayerRepository>();
        gameRepository = new Mock<GameRepository>();

        handler = new LeaveGameCommandHandler(
            playerRepository.Object,
            gameRepository.Object
        );
    }

    private void SetupHappyPath(Game game, LeaveGameCommand command)
    {
        // game exists
        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        // game contains authenticated player
        var player = game.Players.First();
        player.UserId = command.UserId;
        player.Game = game;
    }

    [Fact]
    public async Task GivenGameDoesntExist_WhenHandle_ThenReturnsGameNotFoundError()
    {
        // given
        var command = new LeaveGameCommand("userId", 1);

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
        var command = new LeaveGameCommand("userId", game.Id);

        gameRepository.Setup(r => r.GetById(game.Id, TestContext.Current.CancellationToken))
            .ReturnsAsync(game);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<PlayerNotInGivenGame>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public async Task GivenGameContainingPlayer_WhenHandle_ThenRemovesPlayer(Game game)
    {
        // given
        var command = new LeaveGameCommand("userId", game.Id);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<Success>(result.Value);

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
    }

    [Theory]
    [ClassData(typeof(GameWith2Players))]
    public async Task GivenGameWithTwoPlayers_WhenHandle_ThenRemovesPlayerAndDeletesGame(Game game)
    {
        // given
        var command = new LeaveGameCommand("userId", game.Id);

        SetupHappyPath(game, command);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<Success>(result.Value);

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
        gameRepository.Verify(r => r.Delete(game), Times.Once);
    }
}