using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Lobbies.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Lobbies.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class SendLobbyChatMessageTests : IntegrationTestFixture
{
    private const string lobbyUrl = "Lobbies";

    private static string BuildUrl(long id)
    {
        return $"Lobbies/{id}/messages";
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenUnauthenticatedUser_WhenSendLobbyChatMessage_ThenReturnsUnauthorized(
        SendMessageDto sendMessageDto
    )
    {
        // when
        var response = await Client.PostAsJsonAsync(BuildUrl(1), sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenUserWithoutAccount_WhenSendLobbyChatMessage_ThenReturnsUnauthorized(
        SendMessageDto sendMessageDto
    )
    {
        // when
        var response = await Client.PostAsyncWithUser(BuildUrl(1), TestUser.Default, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenLobbyDoesntExist_WhenSendLobbyChatMessage_ThenReturnsNotFound(
        SendMessageDto sendMessageDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);

        // when
        var response = await Client.PostAsyncWithUser(BuildUrl(1), TestUser.Default, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenPlayerNotInLobby_WhenSendLobbyChatMessage_ThenReturnsNotFound(
        SendMessageDto sendMessageDto
    )
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);

        await Client.PostAsyncWithUser(lobbyUrl,
            TestUser.TestUser1,
            new CreateLobbyDto { Name = "TestLobby", MaxPlayers = 4 }
        );
        var lobbyResponse = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.TestUser1);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        var response = await Client.PostAsyncWithUser(BuildUrl(lobby!.Id), TestUser.Default, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [TestCaseSource(typeof(SendMessageDtoTestCases), nameof(SendMessageDtoTestCases.ValidSendMessageDtoCommands))]
    public async Task GivenPlayerInLobby_WhenSendLobbyChatMessage_ThenReturnsSuccess(
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
        var lobbyResponse = await Client.GetAsyncWithUser(lobbyUrl + "/me", TestUser.Default);
        var lobby = await lobbyResponse.Content.ReadFromJsonAsync<LobbyDto>();

        // when
        var response = await Client.PostAsyncWithUser(BuildUrl(lobby!.Id), TestUser.Default, sendMessageDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}