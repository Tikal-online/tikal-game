using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class SendLobbyChatMessageTests : IntegrationTestFixture
{
    private const string lobbyUrl = "Lobbies";

    private const string sendLobbyChatMessageUrl = "Lobbies/sendMessage";

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenUnauthenticatedUser_WhenSendLobbyChatMessage_ThenReturnsUnauthorized(
        SendMessageDto sendMessageDto
    )
    {
        // when
        var response = await Client.PostAsJsonAsync(sendLobbyChatMessageUrl, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenUserWithoutAccount_WhenSendLobbyChatMessage_ThenReturnsUnauthorized(
        SendMessageDto sendMessageDto
    )
    {
        // when
        var response = await Client.PostAsyncWithUser(sendLobbyChatMessageUrl, TestUser.Default, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenUserNotInALobby_WhenSendLobbyChatMessage_ThenReturnsNotFound(
        SendMessageDto sendMessageDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(sendLobbyChatMessageUrl, TestUser.Default, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenUserInALobby_WhenSendLobbyChatMessage_ThenReturnsSuccess(
        SendMessageDto sendMessageDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await Client.PostAsyncWithUser(
            lobbyUrl,
            TestUser.Default,
            new CreateLobbyDto { Name = "TestLobby", MaxPlayers = 4 }
        );

        // when
        var response = await Client.PostAsyncWithUser(sendLobbyChatMessageUrl, TestUser.Default, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}