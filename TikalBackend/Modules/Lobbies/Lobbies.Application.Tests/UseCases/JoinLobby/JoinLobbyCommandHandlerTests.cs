using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.JoinLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.JoinLobby;

public sealed class JoinLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<LobbyRepository> lobbyRepository;
    private readonly Mock<PlayerQueryContext> playerQueryContext;
    private readonly Mock<UnitOfWork> unitOfWork;
    private readonly AccountContext accountContext;

    // under test
    private readonly JoinLobbyCommandHandler handler;

    public JoinLobbyCommandHandlerTests()
    {
        lobbyRepository = new Mock<LobbyRepository>();
        playerQueryContext = new Mock<PlayerQueryContext>();
        unitOfWork = new Mock<UnitOfWork>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new JoinLobbyCommandHandler(
            lobbyRepository.Object,
            playerQueryContext.Object,
            unitOfWork.Object,
            accountContext
        );
    }

    private void SetupHappyPath(Lobby lobby)
    {
        // player is not in a lobby
        playerQueryContext.Setup(p => p.PlayerExists(accountContext.Account.UserId)).ReturnsAsync(false);

        // lobby exists
        lobbyRepository.Setup(r => r.GetById(lobby.Id)).ReturnsAsync(lobby);
    }

    [Theory]
    [ClassData(typeof(NotFullLobbies))]
    public async Task GivenPlayerIsAlreadyInALobby_WhenHandle_ThenReturnsPlayerAlreadyInALobbyError(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        playerQueryContext.Setup(p => p.PlayerExists(accountContext.Account.UserId)).ReturnsAsync(true);

        var command = new JoinLobbyCommand(lobby.Id);

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
        SetupHappyPath(lobby);

        lobbyRepository.Setup(r => r.GetById(lobby.Id)).ReturnsAsync(default(Lobby));

        var command = new JoinLobbyCommand(lobby.Id);

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
        SetupHappyPath(lobby);

        var command = new JoinLobbyCommand(lobby.Id);

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
        SetupHappyPath(lobby);

        var command = new JoinLobbyCommand(lobby.Id);

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
        SetupHappyPath(lobby);

        var command = new JoinLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        var player = lobby.Players.FirstOrDefault(p => p.UserId == accountContext.Account.UserId);

        Assert.Multiple(
            () => Assert.NotNull(result.Value),
            () => Assert.IsType<Success>(result.Value)
        );
    }
}