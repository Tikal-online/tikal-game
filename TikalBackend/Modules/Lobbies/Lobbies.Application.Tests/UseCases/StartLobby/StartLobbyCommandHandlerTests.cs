using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.StartLobby;
using Moq;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.StartLobby;

internal sealed class StartLobbyCommandHandlerTests
{
    // dependencies
    private Mock<LobbyRepository> lobbyRepository;
    private Mock<UnitOfWork> unitOfWork;
    private AccountContext accountContext;

    // under test
    private StartLobbyCommandHandler handler;

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
}