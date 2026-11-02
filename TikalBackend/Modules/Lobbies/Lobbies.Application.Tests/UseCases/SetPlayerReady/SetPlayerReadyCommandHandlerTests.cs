using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.SetPlayerReady;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;

namespace Lobbies.Application.Tests.UseCases.SetPlayerReady;

public sealed class SetPlayerReadyCommandHandlerTests
{
    // dependencies   
    private readonly Mock<PlayerRepository> playerRepository;

    // under test
    private readonly SetPlayerReadyCommandHandler handler;

    public SetPlayerReadyCommandHandlerTests()
    {
        playerRepository = new Mock<PlayerRepository>();

        handler = new SetPlayerReadyCommandHandler(playerRepository.Object);
    }

    private void SetupHappyPath(Player player)
    {
        // player exists
        playerRepository.Setup(r => r.GetByUserId(player.UserId, TestContext.Current.CancellationToken))
            .ReturnsAsync(player);
    }

    [Fact]
    public async Task GivenNonExistingPlayer_WhenHandle_ThenReturnsPlayerNotInALobbyError()
    {
        // given
        var command = new SetPlayerReadyCommand("userId");

        playerRepository.Setup(r => r.GetByUserId(command.UserId, TestContext.Current.CancellationToken))
            .ReturnsAsync(default(Player));

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.IsType<PlayerNotInALobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidPlayers))]
    public async Task GivenExistingPlayer_WhenHandle_ThenReturnsSuccessAndPlayerIsReady(Player player)
    {
        // given
        var command = new SetPlayerReadyCommand(player.UserId);

        SetupHappyPath(player);

        // when
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // then
        Assert.Multiple(
            () => Assert.IsType<Success>(result.Value),
            () => Assert.True(player.IsReady)
        );
    }
}