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

internal sealed class StartLobbyCommandHandlerTests
{
    // dependencies
    private Mock<LobbyRepository> lobbyRepository;
    private Mock<UnitOfWork> unitOfWork;
    private AccountContext accountContext;

    // under test
    private StartLobbyCommandHandler handler;

    // test data
    public static IEnumerable<Lobby> StartAbleLobbyTests = LobbyTestCases.ValidLobbyTestCases
        .Where(l => l.CanBeStarted).Select(l => l.DeepClone());

    public static IEnumerable<Lobby> NotStartAbleLobbyTests = LobbyTestCases.ValidLobbyTestCases
        .Where(l => !l.CanBeStarted).Select(l => l.DeepClone());

    [SetUp]
    public void Setup()
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
        lobbyRepository.Setup(r => r.GetByIdAsync(lobby.Id))
            .ReturnsAsync(lobby);

        // lobby contains authenticated player who is owner
        var player = lobby.Players.First();
        player.UserId = accountContext.Account.UserId;
        player.Lobby = lobby;
        player.IsOwner = true;
    }

    [Test]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError()
    {
        // given
        lobbyRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(default(Lobby));

        var command = new StartLobbyCommand(1);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<LobbyNotFound>());
    }

    [TestCaseSource(nameof(StartAbleLobbyTests))]
    public async Task GivenPlayerIsNotPartOfLobby_WhenHandle_ThenReturnsPlayerNotInGivenLobbyError(Lobby lobby)
    {
        // given
        lobbyRepository.Setup(r => r.GetByIdAsync(lobby.Id))
            .ReturnsAsync(lobby);

        var command = new StartLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerNotInGivenLobby>());
    }

    [TestCaseSource(nameof(StartAbleLobbyTests))]
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
        Assert.That(result.Value, Is.InstanceOf<Unprivileged>());
    }

    [TestCaseSource(nameof(NotStartAbleLobbyTests))]
    public async Task GivenLobbyIsNotStartAble_WhenHandle_ThenReturnsLobbyCannotBeStartedError(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new StartLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<LobbyCannotBeStarted>());
    }

    [TestCaseSource(nameof(StartAbleLobbyTests))]
    public async Task GivenStartAbleLobby_WhenHandle_ThenStartsLobby(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new StartLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<Success>());

        unitOfWork.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}