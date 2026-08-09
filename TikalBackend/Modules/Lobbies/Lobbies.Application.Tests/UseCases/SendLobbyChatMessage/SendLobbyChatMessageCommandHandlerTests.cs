using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.SendLobbyChatMessage;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
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

    private void SetupHappyPath(Lobby lobby)
    {
        // lobby exists
        lobbyQueryContext.Setup(r => r.GetByIdAsync(lobby.Id))
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
        lobbyQueryContext.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(default(Lobby));

        var command = new SendLobbyChatMessageCommand(1, "My chat message");

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<LobbyNotFound>());
    }

    [TestCaseSource(typeof(LobbyTestCases), nameof(LobbyTestCases.ValidLobbyTestCases))]
    public async Task GivenPlayerIsNotPartOfLobby_WhenHandle_ThenReturnsPlayerNotInGivenLobbyError(Lobby lobby)
    {
        // given
        lobbyQueryContext.Setup(r => r.GetByIdAsync(lobby.Id))
            .ReturnsAsync(lobby);

        var command = new SendLobbyChatMessageCommand(lobby.Id, "My chat message");

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<PlayerNotInGivenLobby>());
    }

    [TestCaseSource(typeof(LobbyTestCases), nameof(LobbyTestCases.ValidLobbyTestCases))]
    public async Task GivenPlayerInLobby_WhenHandle_ThenPublishesMessageSentNotification(Lobby lobby)
    {
        // given
        SetupHappyPath(lobby);

        var command = new SendLobbyChatMessageCommand(lobby.Id, "my chat message");

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