using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.CreateLobby;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Models;
using MediatR;
using Moq;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal sealed class CreateLobbyCommandHandlerTests
{
    // dependencies
    private Mock<LobbyRepository> lobbyRepository;
    private Mock<PlayerQueryContext> playerQueryContext;
    private Mock<ISender> sender;
    private Mock<UnitOfWork> unitOfWork;

    // under test
    private CreateLobbyCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        lobbyRepository = new Mock<LobbyRepository>();
        playerQueryContext = new Mock<PlayerQueryContext>();
        sender = new Mock<ISender>();
        unitOfWork = new Mock<UnitOfWork>();

        handler = new CreateLobbyCommandHandler(
            lobbyRepository.Object,
            playerQueryContext.Object,
            sender.Object,
            unitOfWork.Object
        );
    }

    private void SetupHappyPath(CreateLobbyCommand command)
    {
        // user has an account
        var existingAccount = new AccountModel { UserId = command.UserId, Name = "AccountName" };

        sender.Setup(s => s.Send(
            It.Is<GetAccountQuery>(q => q.UserId == command.UserId),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(existingAccount);

        // player is not in a lobby
        playerQueryContext.Setup(p => p.PlayerExists(command.UserId)).ReturnsAsync(false);
    }

    [TestCaseSource(
        typeof(CreateLobbyCommandTestCases),
        nameof(CreateLobbyCommandTestCases.ValidCreateLobbyCommands)
    )]
    public async Task GivenNoAccountForUserId_WhenHandle_ThenReturnsMissingUserAccountError(CreateLobbyCommand command)
    {
        // given
        SetupHappyPath(command);

        sender.Setup(s => s.Send(
            It.Is<GetAccountQuery>(q => q.UserId == command.UserId),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(default(AccountModel));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<MissingUserAccount>());
    }

    [TestCaseSource(
        typeof(CreateLobbyCommandTestCases),
        nameof(CreateLobbyCommandTestCases.ValidCreateLobbyCommands)
    )]
    public async Task GivenPlayerIsAlreadyInALobby_WhenHandle_ThenReturnsPlayerAlreadyInALobbyError(
        CreateLobbyCommand command
    )
    {
        // given
        SetupHappyPath(command);

        playerQueryContext.Setup(p => p.PlayerExists(command.UserId)).ReturnsAsync(true);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerAlreadyInALobby>());
    }

    [TestCaseSource(
        typeof(CreateLobbyCommandTestCases),
        nameof(CreateLobbyCommandTestCases.ValidCreateLobbyCommands)
    )]
    public async Task GivenSuccessfulCreation_WhenHandle_ThenReturnsCreatedLobby(CreateLobbyCommand command)
    {
        // given
        SetupHappyPath(command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.InstanceOf<LobbyModel>());
            Assert.That(result.AsT0.Name, Is.EqualTo(command.Name));
            Assert.That(result.AsT0.MaxPlayers, Is.EqualTo(command.MaxPlayers));

            Assert.That(result.AsT0.Players.Count, Is.EqualTo(1));
            Assert.That(result.AsT0.Players.First().UserId, Is.EqualTo(command.UserId));
            Assert.That(result.AsT0.Players.First().IsOwner, Is.True);
            Assert.That(result.AsT0.Players.First().IsReady, Is.False);
        }
    }
}