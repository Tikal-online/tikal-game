using Lobbies.Application.DataAccess;
using Lobbies.Application.UseCases.SendLobbyChatMessage;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Tests.Data;
using Moq;

namespace Lobbies.Application.Tests.UseCases.SendLobbyChatMessage;

public sealed class SendLobbyChatMessageCommandHandlerTests
{
    // dependencies
    private readonly Mock<LobbyQueryContext> lobbyQueryContext;

    // under test
    private readonly SendLobbyChatMessageCommandHandler handler;

    public SendLobbyChatMessageCommandHandlerTests()
    {
        lobbyQueryContext = new Mock<LobbyQueryContext>();

        handler = new SendLobbyChatMessageCommandHandler(lobbyQueryContext.Object);
    }

    private void SetupHappyPath(Lobby lobby, SendLobbyChatMessageCommand command)
    {
        // lobby exists
        lobbyQueryContext.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        // lobby contains authenticated player
        var player = lobby.Players.First();
        player.UserId = command.UserId;
        player.Lobby = lobby;
    }

    [Fact]
    public async Task GivenLobbyDoesntExist_WhenHandle_ThenReturnsLobbyNotFoundError()
    {
        // given
        var command = new SendLobbyChatMessageCommand("userId", 1, "My chat message");

        lobbyQueryContext.Setup(r => r.GetById(1))
            .ReturnsAsync(default(Lobby));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<LobbyNotFound>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public async Task GivenPlayerIsNotPartOfLobby_WhenHandle_ThenReturnsPlayerNotInGivenLobbyError(Lobby lobby)
    {
        // given
        var command = new SendLobbyChatMessageCommand("userId", lobby.Id, "My chat message");

        lobbyQueryContext.Setup(r => r.GetById(lobby.Id))
            .ReturnsAsync(lobby);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.IsType<PlayerNotInGivenLobby>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public async Task GivenPlayerInLobby_WhenHandle_ThenReturnsLobbyChatMessageSentNotification(Lobby lobby)
    {
        // given
        var command = new SendLobbyChatMessageCommand("userId", lobby.Id, "My chat message");

        SetupHappyPath(lobby, command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        var expectedPlayer = lobby.Players.First(p => p.UserId == command.UserId);

        Assert.IsType<LobbyChatMessageSentNotification>(result.Value);

        Assert.Multiple(
            () => Assert.Equal(result.AsT0.ChatMessageModel.Content, command.MessageContent),
            () => Assert.Equal(result.AsT0.ChatMessageModel.UserId, expectedPlayer.UserId),
            () => Assert.Equal(result.AsT0.ChatMessageModel.Username, expectedPlayer.Name)
        );
    }
}