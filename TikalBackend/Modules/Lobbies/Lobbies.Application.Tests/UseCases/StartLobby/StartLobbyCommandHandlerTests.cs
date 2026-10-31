using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.StartLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;
using Shared.Contracts.Errors;

namespace Lobbies.Application.Tests.UseCases.StartLobby;

public sealed class StartLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<LobbyRepository> lobbyRepository;

    // under test
    private readonly StartLobbyCommandHandler handler;

    public StartLobbyCommandHandlerTests()
    {
        lobbyRepository = new Mock<LobbyRepository>();

        handler = new StartLobbyCommandHandler(lobbyRepository.Object);
    }

    private void SetupHappyPath(Lobby lobby, StartLobbyCommand command)
    {
        // lobby exists
        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        // lobby contains authenticated player who is owner
        var player = lobby.Players.First();
        player.UserId = command.UserId;
        player.Lobby = lobby;
        player.IsOwner = true;
    }

    [Fact]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError()
    {
        // given
        var command = new StartLobbyCommand("userId", 1);

        lobbyRepository.Setup(r => r.GetById(1))
            .ReturnsAsync(default(Lobby));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyNotFound>(result.Value);
    }

    [Theory]
    [ClassData(typeof(StartableLobbies))]
    public async Task GivenPlayerIsNotPartOfLobby_WhenHandle_ThenReturnsPlayerNotInGivenLobbyError(Lobby lobby)
    {
        // given
        var command = new StartLobbyCommand("userId", lobby.Id);

        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<PlayerNotInGivenLobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(StartableLobbies))]
    public async Task GivenPlayerIsNotLobbyOwner_WhenHandle_ThenReturnsUnprivilegedError(Lobby lobby)
    {
        // given
        var command = new StartLobbyCommand("userId", lobby.Id);

        SetupHappyPath(lobby, command);

        var player = lobby.GetPlayer(command.UserId);
        player?.IsOwner = false;

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Unprivileged>(result.Value);
    }

    [Theory]
    [ClassData(typeof(NotStartableLobbies))]
    public async Task GivenLobbyIsNotStartAble_WhenHandle_ThenReturnsLobbyCannotBeStartedError(Lobby lobby)
    {
        // given
        var command = new StartLobbyCommand("userId", lobby.Id);

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyCannotBeStarted>(result.Value);
    }

    [Theory]
    [ClassData(typeof(StartableLobbies))]
    public async Task GivenStartAbleLobby_WhenHandle_ThenReturnsSuccess(Lobby lobby)
    {
        // given
        var command = new StartLobbyCommand("userId", lobby.Id);

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);
    }
}