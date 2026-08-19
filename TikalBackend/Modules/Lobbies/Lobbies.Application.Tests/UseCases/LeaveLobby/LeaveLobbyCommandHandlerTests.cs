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
using Shared.Application.Tests.Extensions;

namespace Lobbies.Application.Tests.UseCases.LeaveLobby;

internal sealed class LeaveLobbyCommandHandlerTests
{
    // dependencies
    private Mock<PlayerRepository> playerRepository;
    private Mock<LobbyRepository> lobbyRepository;
    private Mock<UnitOfWork> unitOfWork;
    private AccountContext accountContext;

    // under test
    private LeaveLobbyCommandHandler handler;

    // test data
    public static IEnumerable<Lobby> LobbyWithMoreThanOnePlayerTests => LobbyTestCases.ValidLobbyTestCases
        .Where(l => l.Players.Count > 1 && !l.InGame)
        .Select(l => l.DeepClone());

    public static IEnumerable<Lobby> LobbyWithOnePlayerTests => LobbyTestCases.ValidLobbyTestCases
        .Where(l => l.Players.Count == 1 && !l.InGame)
        .Select(l => l.DeepClone());

    public static IEnumerable<Lobby> InGameLobbyTests => LobbyTestCases.ValidLobbyTestCases
        .Where(l => l.InGame)
        .Select(l => l.DeepClone());

    [SetUp]
    public void Setup()
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

    [Test]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError()
    {
        // given
        lobbyRepository.Setup(r => r.GetById(1))
            .ReturnsAsync(default(Lobby));

        var command = new LeaveLobbyCommand(1);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<LobbyNotFound>());
    }

    [TestCaseSource(nameof(LobbyWithMoreThanOnePlayerTests))]
    public async Task GivenPlayerIsNotPartOfLobby_WhenHandle_ThenReturnsPlayerNotInGivenLobbyError(Lobby lobby)
    {
        // given
        lobbyRepository.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerNotInGivenLobby>());
    }

    [TestCaseSource(nameof(InGameLobbyTests))]
    public async Task GivenLobbyInGame_WhenHandle_ThenReturnsLobbyInGameError(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<LobbyInGame>());
    }

    [TestCaseSource(nameof(LobbyWithMoreThanOnePlayerTests))]
    public async Task GivenLobbyWithMultiplePlayers_WhenHandle_ThenRemovesPlayer(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<Success>());

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [TestCaseSource(nameof(LobbyWithOnePlayerTests))]
    public async Task GivenLobbyWithOnePlayer_WhenHandle_ThenRemovesPlayerAndDeletesLobby(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new LeaveLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<Success>());

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
        lobbyRepository.Verify(r => r.Delete(lobby), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}