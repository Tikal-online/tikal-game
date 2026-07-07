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

internal sealed class JoinLobbyCommandHandlerTests
{
    // dependencies
    private Mock<LobbyRepository> lobbyRepository;
    private Mock<PlayerQueryContext> playerQueryContext;
    private Mock<UnitOfWork> unitOfWork;
    private AccountContext accountContext;

    // under test
    private JoinLobbyCommandHandler handler;

    // test data
    public static IEnumerable<Lobby> NotFullLobbyTestCases => LobbyTestCases.ValidLobbyTestCases.Where(l => !l.IsFull);

    public static IEnumerable<Lobby> FullLobbyTestCases => LobbyTestCases.ValidLobbyTestCases.Where(l => l.IsFull);

    [SetUp]
    public void Setup()
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
        lobbyRepository.Setup(r => r.GetByIdAsync(lobby.Id)).ReturnsAsync(lobby);
    }

    [TestCaseSource(nameof(NotFullLobbyTestCases))]
    public async Task GivenPlayerIsAlreadyInALobby_WhenHandle_ThenReturnsPlayerAlreadyInALobbyError(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        playerQueryContext.Setup(p => p.PlayerExists(accountContext.Account.UserId)).ReturnsAsync(true);

        var command = new JoinLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerAlreadyInALobby>());
    }

    [TestCaseSource(nameof(NotFullLobbyTestCases))]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        lobbyRepository.Setup(r => r.GetByIdAsync(lobby.Id)).ReturnsAsync(default(Lobby));

        var command = new JoinLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<LobbyNotFound>());
    }

    [TestCaseSource(nameof(FullLobbyTestCases))]
    public async Task GivenFullLobby_WhenHandle_ThenReturnsLobbyFullError(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new JoinLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<LobbyFull>());
    }

    [TestCaseSource(nameof(NotFullLobbyTestCases))]
    public async Task GivenNotFullLobby_WhenHandle_ThenAddsPlayerToLobby(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new JoinLobbyCommand(lobby.Id);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        var player = lobby.Players.FirstOrDefault(p => p.UserId == accountContext.Account.UserId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.InstanceOf<Success>());
            Assert.That(player, Is.Not.Null);
        }
    }
}