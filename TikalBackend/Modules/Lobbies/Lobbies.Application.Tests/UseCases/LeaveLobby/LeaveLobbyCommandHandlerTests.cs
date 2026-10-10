using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.LeaveLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.LeaveLobby;

public sealed class LeaveLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<PlayerRepository> playerRepository;
    private readonly Mock<LobbyRepository> lobbyRepository;
    private readonly Mock<UnitOfWork> unitOfWork;
    private readonly AccountContext accountContext;

    // under test
    private readonly LeaveLobbyCommandHandler handler;

    public LeaveLobbyCommandHandlerTests()
    {
        playerRepository = new Mock<PlayerRepository>();
        lobbyRepository = new Mock<LobbyRepository>();
        unitOfWork = new Mock<UnitOfWork>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new LeaveLobbyCommandHandler(
            playerRepository.Object,
            lobbyRepository.Object,
            unitOfWork.Object,
            accountContext
        );
    }

    private void SetupHappyPath(Lobby lobby)
    {
        // lobby exists
        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        // lobby contains authenticated player
        var player = lobby.Players.First();
        player.UserId = accountContext.Account.UserId;
        player.Lobby = lobby;
    }

    [Fact]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError()
    {
        // given
        lobbyRepository.Setup(r => r.GetById(1))
            .ReturnsAsync(default(Lobby));

        var command = new LeaveLobbyCommand(1);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyNotFound>(result.Value);
    }

    [Theory]
    [ClassData(typeof(LobbiesWithMoreThanOnePlayer))]
    public async Task GivenPlayerIsNotPartOfLobby_WhenHandle_ThenReturnsPlayerNotInGivenLobbyError(Lobby lobby)
    {
        // given
        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<PlayerNotInGivenLobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(InGameLobbies))]
    public async Task GivenLobbyInGame_WhenHandle_ThenReturnsLobbyInGameError(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyInGame>(result.Value);
    }

    [Theory]
    [ClassData(typeof(LobbiesWithMoreThanOnePlayer))]
    public async Task GivenLobbyWithMultiplePlayers_WhenHandle_ThenRemovesPlayer(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Theory]
    [ClassData(typeof(LobbiesWithOnePlayer))]
    public async Task GivenLobbyWithOnePlayer_WhenHandle_ThenRemovesPlayerAndDeletesLobby(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
        lobbyRepository.Verify(r => r.Delete(lobby), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}