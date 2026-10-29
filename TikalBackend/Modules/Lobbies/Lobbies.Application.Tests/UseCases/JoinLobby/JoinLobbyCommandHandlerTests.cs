using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.JoinLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;

namespace Lobbies.Application.Tests.UseCases.JoinLobby;

public sealed class JoinLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<LobbyRepository> lobbyRepository;
    private readonly Mock<PlayerQueryContext> playerQueryContext;

    // under test
    private readonly JoinLobbyCommandHandler handler;

    public JoinLobbyCommandHandlerTests()
    {
        lobbyRepository = new Mock<LobbyRepository>();
        playerQueryContext = new Mock<PlayerQueryContext>();

        handler = new JoinLobbyCommandHandler(
            lobbyRepository.Object,
            playerQueryContext.Object
        );
    }

    private void SetupHappyPath(Lobby lobby, JoinLobbyCommand command)
    {
        // player is not in a lobby
        playerQueryContext.Setup(p => p.PlayerExists(command.UserId)).ReturnsAsync(false);

        // lobby exists
        lobbyRepository.Setup(r => r.GetById(lobby.Id)).ReturnsAsync(lobby);
    }

    [Theory]
    [ClassData(typeof(NotFullLobbies))]
    public async Task GivenPlayerIsAlreadyInALobby_WhenHandle_ThenReturnsPlayerAlreadyInALobbyError(Lobby lobby)
    {
        // given
        var command = new JoinLobbyCommand("userId", "username", lobby.Id);

        SetupHappyPath(lobby, command);

        playerQueryContext.Setup(p => p.PlayerExists(command.UserId)).ReturnsAsync(true);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<PlayerAlreadyInALobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(NotFullLobbies))]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError(Lobby lobby)
    {
        // given
        var command = new JoinLobbyCommand("userId", "username", lobby.Id);

        SetupHappyPath(lobby, command);

        lobbyRepository.Setup(r => r.GetById(lobby.Id)).ReturnsAsync(default(Lobby));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyNotFound>(result.Value);
    }

    [Theory]
    [ClassData(typeof(FullLobbies))]
    public async Task GivenFullLobby_WhenHandle_ThenReturnsLobbyFullError(Lobby lobby)
    {
        // given
        var command = new JoinLobbyCommand("userId", "username", lobby.Id);

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyFull>(result.Value);
    }

    [Theory]
    [ClassData(typeof(InGameLobbies))]
    public async Task GivenLobbyInGame_WhenHandle_ThenReturnsLobbyInGameError(Lobby lobby)
    {
        // given
        var command = new JoinLobbyCommand("userId", "username", lobby.Id);

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyInGame>(result.Value);
    }

    [Theory]
    [ClassData(typeof(NotFullLobbies))]
    public async Task GivenNotFullLobby_WhenHandle_ThenAddsPlayerToLobby(Lobby lobby)
    {
        // given
        var command = new JoinLobbyCommand("userId", "username", lobby.Id);

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        var player = lobby.Players.FirstOrDefault(p => p.UserId == command.UserId);

        Assert.Multiple(
            () => Assert.NotNull(result.Value),
            () => Assert.IsType<Success>(result.Value)
        );
    }
}