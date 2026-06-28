using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using RestApi.Controllers.Lobbies.Dtos;
using Shared.Contracts.Queries;
using TikalBackend.IntegrationTests.Extensions;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

internal sealed class GetPaginatedLobbiesTests : IntegrationTestFixture
{
    private const string lobbyUrl = "Lobbies";

    [Test]
    public async Task GivenUnauthenticatedUser_WhenGetPaginatedLobbies_ThenReturnsUnauthorized()
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageSize"] = "10",
            ["pageNumber"] = "1",
            ["searchText"] = ""
        };

        var url = QueryHelpers.AddQueryString(lobbyUrl, queryParams);

        // when
        var response = await Client.GetAsync(url);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenUserWithoutAccount_WhenGetPaginatedLobbies_ThenReturnsUnauthorized()
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageSize"] = "10",
            ["pageNumber"] = "1",
            ["searchText"] = ""
        };

        var url = QueryHelpers.AddQueryString(lobbyUrl, queryParams);

        // when
        var response = await Client.GetAsyncWithUser(url, TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // TODO: improve this test by parameterizing it to test multiple scenarios
    [Test]
    public async Task GivenLobbiesAndSearchString_WhenGetPaginatedLobbies_ThenReturnsLobbiesWithMatchingNames()
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);
        await CreateUserAccount(TestUser.TestUser2);

        await Client.PostAsyncWithUser(
            lobbyUrl,
            TestUser.Default,
            new CreateLobbyDto
            {
                Name = "Lobby1",
                MaxPlayers = 2
            }
        );

        await Client.PostAsyncWithUser(
            lobbyUrl,
            TestUser.TestUser1,
            new CreateLobbyDto
            {
                Name = "Lobby2",
                MaxPlayers = 3
            }
        );

        await Client.PostAsyncWithUser(
            lobbyUrl,
            TestUser.TestUser2,
            new CreateLobbyDto
            {
                Name = "Lobby3",
                MaxPlayers = 4
            }
        );

        var queryParams = new Dictionary<string, string?>
        {
            ["pageSize"] = "10",
            ["pageNumber"] = "1",
            ["searchText"] = "2"
        };

        var url = QueryHelpers.AddQueryString(lobbyUrl, queryParams);

        // when
        var response = await Client.GetAsyncWithUser(url, TestUser.Default);

        var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedResult<List<LobbySummaryDto>>>();

        var lobbies = paginatedResult?.Data;

        // then
        Assert.That(lobbies, Is.Not.Null);
        Assert.That(lobbies, Has.Count.EqualTo(1));
        Assert.That(lobbies.First().Name, Is.EqualTo("Lobby2"));
    }
}