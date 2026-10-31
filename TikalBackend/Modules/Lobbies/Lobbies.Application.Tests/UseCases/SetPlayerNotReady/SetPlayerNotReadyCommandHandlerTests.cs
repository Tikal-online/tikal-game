using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.SetPlayerNotReady;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;

namespace Lobbies.Application.Tests.UseCases.SetPlayerNotReady;

public sealed class SetPlayerNotReadyCommandHandlerTests
{
    // dependencies
    private readonly Mock<PlayerRepository> playerRepository;

    // under test
    private readonly SetPlayerNotReadyCommandHandler handler;

    public SetPlayerNotReadyCommandHandlerTests()
    {
        playerRepository = new Mock<PlayerRepository>();

        handler = new SetPlayerNotReadyCommandHandler(playerRepository.Object);
    }

    private void SetupHappyPath(Player player)
    {
        // player exists
        playerRepository.Setup(r => r.GetByUserId(player.UserId))
            .ReturnsAsync(player);
    }

    [Fact]
    public async Task GivenNonExistingPlayer_WhenHandle_ThenReturnsPlayerNotInALobbyError()
    {
        // given
        var command = new SetPlayerNotReadyCommand("userId");

        playerRepository.Setup(r => r.GetByUserId(command.UserId))
            .ReturnsAsync(default(Player));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<PlayerNotInALobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidPlayers))]
    public async Task GivenExistingPlayer_WhenHandle_ThenReturnsSuccessAndPlayerIsNotReady(Player player)
    {
        // given
        var command = new SetPlayerNotReadyCommand(player.UserId);

        SetupHappyPath(player);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.IsType<Success>(result.Value),
            () => Assert.False(player.IsReady)
        );
    }
}