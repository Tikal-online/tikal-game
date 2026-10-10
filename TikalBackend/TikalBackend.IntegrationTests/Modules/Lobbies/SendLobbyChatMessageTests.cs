using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class SendLobbyChatMessageTests : IntegrationTestFixture
{
    [Theory]
    [ClassData(typeof(ValidSendMessageDtos))]
    public async Task GivenUnauthenticatedUser_WhenSendLobbyChatMessage_ThenReturnsUnauthorized(
        SendMessageDto sendMessageDto
    )
    {
        // when
        var response = await Client.PostAsJsonAsync(LobbyUrl.SendMessage(1), sendMessageDto, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidSendMessageDtos))]
    public async Task GivenUserWithoutAccount_WhenSendLobbyChatMessage_ThenReturnsUnauthorized(
        SendMessageDto sendMessageDto
    )
    {
        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.SendMessage(1), TestUser.Default, sendMessageDto, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidSendMessageDtos))]
    public async Task GivenLobbyDoesntExist_WhenSendLobbyChatMessage_ThenReturnsNotFound(
        SendMessageDto sendMessageDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.SendMessage(1), TestUser.Default, sendMessageDto, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidSendMessageDtos))]
    public async Task GivenPlayerNotInLobby_WhenSendLobbyChatMessage_ThenReturnsNotFound(
        SendMessageDto sendMessageDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        var createLobbyDto = new CreateLobbyDto { Name = "TestLobby", MaxPlayers = 4 };

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.TestUser1);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.SendMessage(lobby.Id), TestUser.Default, sendMessageDto, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidSendMessageDtos))]
    public async Task GivenPlayerInLobby_WhenSendLobbyChatMessage_ThenReturnsSuccess(
        SendMessageDto sendMessageDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        var createLobbyDto = new CreateLobbyDto { Name = "TestLobby", MaxPlayers = 4 };

        var lobby = await CreateAndGetLobby(createLobbyDto, TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(LobbyUrl.SendMessage(lobby.Id), TestUser.Default, sendMessageDto, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}