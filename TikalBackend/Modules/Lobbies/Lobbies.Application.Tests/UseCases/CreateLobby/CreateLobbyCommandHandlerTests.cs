using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.CreateLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Moq;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal sealed class CreateLobbyCommandHandlerTests
{
    // dependencies
    private Mock<LobbyRepository> lobbyRepository;
    private Mock<PlayerQueryContext> playerQueryContext;
    private Mock<UnitOfWork> unitOfWork;
    private AccountContext accountContext;

    // under test
    private CreateLobbyCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        lobbyRepository = new Mock<LobbyRepository>();
        playerQueryContext = new Mock<PlayerQueryContext>();
        unitOfWork = new Mock<UnitOfWork>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new CreateLobbyCommandHandler(
            lobbyRepository.Object,
            playerQueryContext.Object,
            unitOfWork.Object,
            accountContext
        );
    }

    private void SetupHappyPath()
    {
        // player is not in a lobby
        playerQueryContext.Setup(p => p.PlayerExists(accountContext.Account.UserId)).ReturnsAsync(false);
    }

    [TestCaseSource(
        typeof(CreateLobbyCommandTestCases),
        nameof(CreateLobbyCommandTestCases.ValidCreateLobbyCommands)
    )]
    public async Task GivenPlayerIsAlreadyInALobby_WhenHandle_ThenReturnsPlayerAlreadyInALobbyError(
        CreateLobbyCommand command
    )
    {
        // given
        SetupHappyPath();

        playerQueryContext.Setup(p => p.PlayerExists(accountContext.Account.UserId)).ReturnsAsync(true);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerAlreadyInALobby>());
    }

    [TestCaseSource(
        typeof(CreateLobbyCommandTestCases),
        nameof(CreateLobbyCommandTestCases.ValidCreateLobbyCommands)
    )]
    public async Task GivenSuccessfulCreation_WhenHandle_ThenReturnsSuccess(CreateLobbyCommand command)
    {
        // given
        SetupHappyPath();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<Success>());
    }
}