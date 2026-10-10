using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.StartLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Application.Tests;
using Shared.Application.Tests.Extensions;
using Shared.Contracts.Errors;

namespace Lobbies.Application.Tests.UseCases.StartLobby;

public sealed class StartLobbyCommandHandlerTests
{
    // dependencies
    private readonly Mock<LobbyRepository> lobbyRepository;
    private readonly Mock<UnitOfWork> unitOfWork;
    private readonly AccountContext accountContext;

    // under test
    private readonly StartLobbyCommandHandler handler;

    public StartLobbyCommandHandlerTests()
    {
        lobbyRepository = new Mock<LobbyRepository>();
        unitOfWork = new Mock<UnitOfWork>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new StartLobbyCommandHandler(
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

        // lobby contains authenticated player who is owner
        var player = lobby.Players.First();
        player.UserId = accountContext.Account.UserId;
        player.Lobby = lobby;
        player.IsOwner = true;
    }

    [Fact]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError()
    {
        // given
        lobbyRepository.Setup(r => r.GetById(1))
            .ReturnsAsync(default(Lobby));

        var command = new StartLobbyCommand(1);

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
        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        var command = new StartLobbyCommand(lobby.Id);

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
        SetupHappyPath(lobby);

        var player = lobby.GetPlayer(accountContext.Account.UserId);
        player?.IsOwner = false;

        var command = new StartLobbyCommand(lobby.Id);

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
        SetupHappyPath(lobby);

        var command = new StartLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyCannotBeStarted>(result.Value);
    }

    [Theory]
    [ClassData(typeof(StartableLobbies))]
    public async Task GivenStartAbleLobby_WhenHandle_ThenStartsLobby(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new StartLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<Success>(result.Value);

        unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}