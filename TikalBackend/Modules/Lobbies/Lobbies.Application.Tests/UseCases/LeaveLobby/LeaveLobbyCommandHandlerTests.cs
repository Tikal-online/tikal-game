using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.LeaveLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;

namespace Lobbies.Application.Tests.UseCases.LeaveLobby;

public sealed class LeaveLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<PlayerRepository> playerRepository;
    private readonly Mock<LobbyRepository> lobbyRepository;

    // under test
    private readonly LeaveLobbyCommandHandler handler;

    public LeaveLobbyCommandHandlerTests()
    {
        playerRepository = new Mock<PlayerRepository>();
        lobbyRepository = new Mock<LobbyRepository>();

        handler = new LeaveLobbyCommandHandler(
            playerRepository.Object,
            lobbyRepository.Object
        );
    }

    private void SetupHappyPath(Lobby lobby, LeaveLobbyCommand command)
    {
        // lobby exists
        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        // lobby contains authenticated player
        var player = lobby.Players.First();
        player.UserId = command.UserId;
        player.Lobby = lobby;
    }

    [Fact]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError()
    {
        // given
        var command = new LeaveLobbyCommand("userId", 1);

        lobbyRepository.Setup(r => r.GetById(1))
            .ReturnsAsync(default(Lobby));

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
        var command = new LeaveLobbyCommand("userId", lobby.Id);

        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

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
        var command = new LeaveLobbyCommand("userId", lobby.Id);

        SetupHappyPath(lobby, command);

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
        var command = new LeaveLobbyCommand("userId", lobby.Id);

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
    }

    [Theory]
    [ClassData(typeof(LobbiesWithOnePlayer))]
    public async Task GivenLobbyWithOnePlayer_WhenHandle_ThenRemovesPlayerAndDeletesLobby(Lobby lobby)
    {
        // given
        var command = new LeaveLobbyCommand("userId", lobby.Id);

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
        lobbyRepository.Verify(r => r.Delete(lobby), Times.Once);
    }
}