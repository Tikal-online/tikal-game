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

internal sealed class SetPlayerReadyCommandHandlerTests
{
    // dependencies   
    private Mock<PlayerRepository> playerRepository;
    private Mock<UnitOfWork> unitOfWork;
    private AccountContext accountContext;

    // under test
    private SetPlayerReadyCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        playerRepository = new Mock<PlayerRepository>();
        unitOfWork = new Mock<UnitOfWork>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new SetPlayerReadyCommandHandler(playerRepository.Object, unitOfWork.Object, accountContext);
    }

    private void SetupHappyPath(Player player)
    {
        // player exists
        playerRepository.Setup(r => r.GetByUserIdAsync(accountContext.Account.UserId))
            .ReturnsAsync(player);
    }

    [Test]
    public async Task GivenNonExistingPlayer_WhenHandle_ThenReturnsPlayerNotInALobbyError()
    {
        // given
        playerRepository.Setup(r => r.GetByUserIdAsync(accountContext.Account.UserId))
            .ReturnsAsync(default(Player));

        var command = new SetPlayerReadyCommand();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerNotInALobby>());
    }

    [TestCaseSource(typeof(PlayerTestCases), nameof(PlayerTestCases.ValidPlayerTestCases))]
    public async Task GivenExistingPlayer_WhenHandle_ThenReturnsSuccessAndPlayerIsReady(Player player)
    {
        // given
        SetupHappyPath(player);

        var command = new SetPlayerReadyCommand();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.InstanceOf<Success>());
            Assert.That(player.IsReady, Is.True);
        }
    }
}