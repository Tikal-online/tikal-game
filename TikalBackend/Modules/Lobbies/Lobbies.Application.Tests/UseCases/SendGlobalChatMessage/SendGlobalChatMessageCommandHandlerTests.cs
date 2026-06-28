using Lobbies.Application.UseCases.SendGlobalChatMessage;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Notifications;
using MediatR;
using Moq;
using Shared.Application.Contexts;
using Shared.Application.Tests;

namespace Lobbies.Application.Tests.UseCases.SendGlobalChatMessage;

internal sealed class SendGlobalChatMessageCommandHandlerTests
{
    // dependencies
    private Mock<IPublisher> publisher;
    private AccountContext accountContext;

    // under test
    private SendGlobalChatMessageCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        publisher = new Mock<IPublisher>();
        accountContext = AccountContextHelper.TestAccountContext;

        handler = new SendGlobalChatMessageCommandHandler(accountContext, publisher.Object);
    }

    [TestCaseSource(
        typeof(SendGlobalChatMessageCommandTestCases),
        nameof(SendGlobalChatMessageCommandTestCases.ValidSendGlobalMessageCommands)
    )]
    public async Task GivenValidCommand_WhenHandle_ThenPublishesMessageSentNotification(
        SendGlobalChatMessageCommand command
    )
    {
        // when
        await handler.Handle(command, CancellationToken.None);

        // then
        publisher.Verify(p =>
            p.Publish(
                It.Is<GlobalChatMessageSentNotification>(n =>
                    n.message.Content == command.messageContent &&
                    n.message.UserId == accountContext.Account.UserId &&
                    n.message.Username == accountContext.Account.Name
                )
            ));
    }
}