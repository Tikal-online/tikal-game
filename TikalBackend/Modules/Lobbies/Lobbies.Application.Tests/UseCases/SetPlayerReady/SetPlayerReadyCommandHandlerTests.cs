using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.SetPlayerReady;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.SetPlayerReady;

public sealed class SetPlayerReadyCommandHandlerTests
{
    // dependencies   
    private readonly Mock<PlayerRepository> playerRepository;
    private readonly Mock<UnitOfWork> unitOfWork;
    private readonly AccountContext accountContext;

    // under test
    private readonly SetPlayerReadyCommandHandler handler;

    public SetPlayerReadyCommandHandlerTests()
    {
        playerRepository = new Mock<PlayerRepository>();
        unitOfWork = new Mock<UnitOfWork>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new SetPlayerReadyCommandHandler(playerRepository.Object, unitOfWork.Object, accountContext);
    }

    private void SetupHappyPath(Player player)
    {
        // player exists
        playerRepository.Setup(r => r.GetByUserId(accountContext.Account.UserId))
            .ReturnsAsync(player);
    }

    [Fact]
    public async Task GivenNonExistingPlayer_WhenHandle_ThenReturnsPlayerNotInALobbyError()
    {
        // given
        playerRepository.Setup(r => r.GetByUserId(accountContext.Account.UserId))
            .ReturnsAsync(default(Player));

        var command = new SetPlayerReadyCommand();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<PlayerNotInALobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidPlayers))]
    public async Task GivenExistingPlayer_WhenHandle_ThenReturnsSuccessAndPlayerIsReady(Player player)
    {
        // given
        SetupHappyPath(player);

        var command = new SetPlayerReadyCommand();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.IsType<Success>(result.Value),
            () => Assert.True(player.IsReady)
        );
    }
}