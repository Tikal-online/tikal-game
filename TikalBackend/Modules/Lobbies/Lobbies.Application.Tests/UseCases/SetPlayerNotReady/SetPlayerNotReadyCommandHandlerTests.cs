using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.SetPlayerNotReady;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.SetPlayerNotReady;

public sealed class SetPlayerNotReadyCommandHandlerTests
{
    // dependencies
    private readonly Mock<PlayerRepository> playerRepository;
    private readonly Mock<UnitOfWork> unitOfWork;
    private readonly AccountContext accountContext;

    // under test
    private readonly SetPlayerNotReadyCommandHandler handler;

    public SetPlayerNotReadyCommandHandlerTests()
    {
        playerRepository = new Mock<PlayerRepository>();
        unitOfWork = new Mock<UnitOfWork>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new SetPlayerNotReadyCommandHandler(playerRepository.Object, unitOfWork.Object, accountContext);
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

        var command = new SetPlayerNotReadyCommand();

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
        SetupHappyPath(player);

        var command = new SetPlayerNotReadyCommand();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.IsType<Success>(result.Value),
            () => Assert.False(player.IsReady)
        );
    }
}