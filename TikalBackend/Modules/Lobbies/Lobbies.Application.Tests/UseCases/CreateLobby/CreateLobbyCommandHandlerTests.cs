using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.CreateLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Moq;
using OneOf.Types;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

public sealed class CreateLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<LobbyRepository> lobbyRepository;
    private readonly Mock<PlayerQueryContext> playerQueryContext;

    // under test
    private readonly CreateLobbyCommandHandler handler;

    public CreateLobbyCommandHandlerTests()
    {
        lobbyRepository = new Mock<LobbyRepository>();
        playerQueryContext = new Mock<PlayerQueryContext>();

        handler = new CreateLobbyCommandHandler(
            lobbyRepository.Object,
            playerQueryContext.Object
        );
    }

    private void SetupHappyPath(CreateLobbyCommand command)
    {
        // player is not in a lobby
        playerQueryContext.Setup(p => p.PlayerExists(command.UserId)).ReturnsAsync(false);
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyCommands))]
    public async Task GivenPlayerIsAlreadyInALobby_WhenHandle_ThenReturnsPlayerAlreadyInALobbyError(
        CreateLobbyCommand command
    )
    {
        // given
        playerQueryContext.Setup(p => p.PlayerExists(command.UserId)).ReturnsAsync(true);

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
        SetupHappyPath(command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);
    }
}