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
        .Where(l => l.Players.Count > 1);

    public static IEnumerable<Lobby> LobbyWithOnePlayerTests => LobbyTestCases.ValidLobbyTestCases
        .Where(l => l.Players.Count == 1);

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
        // player exists
        var player = lobby.Players.First();
        player.Lobby = lobby;

        playerRepository.Setup(r => r.GetByUserIdWithLobbyAsync(accountContext.Account.UserId))
            .ReturnsAsync(player);
    }

    [Test]
    public async Task GivenPlayerDoesntExist_WhenHandle_ThenReturnsPlayerNotInALobbyError()
    {
        // given
        playerRepository.Setup(r => r.GetByUserIdWithLobbyAsync(accountContext.Account.UserId))
            .ReturnsAsync(default(Player));

        var command = new LeaveLobbyCommand();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerNotInALobby>());
    }

    [TestCaseSource(nameof(LobbyWithMoreThanOnePlayerTests))]
    public async Task GivenLobbyWithMultiplePlayers_WhenHandle_ThenRemovesPlayer(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new LeaveLobbyCommand();

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

        var command = new LeaveLobbyCommand();

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<Success>());

        playerRepository.Verify(r => r.Delete(It.IsAny<Player>()), Times.Once);
        lobbyRepository.Verify(r => r.Delete(lobby), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}