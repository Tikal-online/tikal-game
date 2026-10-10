using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.CreateLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Moq;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

public sealed class CreateLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<LobbyRepository> lobbyRepository;
    private readonly Mock<PlayerQueryContext> playerQueryContext;
    private readonly Mock<UnitOfWork> unitOfWork;
    private readonly AccountContext accountContext;

    // under test
    private readonly CreateLobbyCommandHandler handler;

    public CreateLobbyCommandHandlerTests()
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

    [Theory]
    [ClassData(typeof(ValidCreateLobbyCommands))]
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
        Assert.IsType<PlayerAlreadyInALobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyCommands))]
    public async Task GivenSuccessfulCreation_WhenHandle_ThenReturnsSuccess(CreateLobbyCommand command)
    {
        // given
        SetupHappyPath();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);
    }
}