using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.SendLobbyChatMessage;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Notifications;
using MediatR;
using Moq;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.SendLobbyChatMessage;

internal sealed class SendLobbyChatMessageCommandHandlerTests
{
    // dependencies
    private Mock<LobbyQueryContext> lobbyQueryContext;
    private Mock<IPublisher> publisher;
    private AccountContext accountContext;

    // under test
    private SendLobbyChatMessageCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        lobbyQueryContext = new Mock<LobbyQueryContext>();
        publisher = new Mock<IPublisher>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new SendLobbyChatMessageCommandHandler(lobbyQueryContext.Object, publisher.Object, accountContext);
    }

    private void SetupHappyPath()
    {
        // player is in a lobby
        lobbyQueryContext.Setup(q => q.GetIdByUserIdAsync(accountContext.Account.UserId))
            .ReturnsAsync(1);
    }


    [TestCaseSource(
        typeof(SendLobbyChatMessageCommandTestCases),
        nameof(SendLobbyChatMessageCommandTestCases.ValidSendLobbyMessageCommands)
    )]
    public async Task GivenPlayerNotInALobby_WhenHandle_ThenReturnsPlayerNotInALobbyError(
        SendLobbyChatMessageCommand command
    )
    {
        // given
        lobbyQueryContext.Setup(q => q.GetIdByUserIdAsync(accountContext.Account.UserId))
            .ReturnsAsync((long?)null);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerNotInALobby>());
    }

    [TestCaseSource(
        typeof(SendLobbyChatMessageCommandTestCases),
        nameof(SendLobbyChatMessageCommandTestCases.ValidSendLobbyMessageCommands)
    )]
    public async Task GivenPlayerInALobby_WhenHandle_ThenPublishesMessageSentNotification(
        SendLobbyChatMessageCommand command
    )
    {
        // given
        SetupHappyPath();

        // when
        await handler.Handle(command, CancellationToken.None);

        // then
        publisher.Verify(p => p.Publish(It.Is<LobbyChatMessageSentNotification>(n =>
            n.ChatMessageModel.Content == command.MessageContent &&
            n.ChatMessageModel.UserId == accountContext.Account.UserId &&
            n.ChatMessageModel.Username == accountContext.Account.Name
        )));
    }
}