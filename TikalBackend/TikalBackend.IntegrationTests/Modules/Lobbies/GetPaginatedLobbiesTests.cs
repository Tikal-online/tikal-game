using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using RestApi.Controllers.Lobbies.Dtos;
using Shared.Contracts.Queries;
using TikalBackend.IntegrationTests.Extensions;

namespace TikalBackend.IntegrationTests.Modules.Lobbies;

public sealed class GetPaginatedLobbiesTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGetPaginatedLobbies_ThenReturnsUnauthorized()
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageSize"] = "10",
            ["pageNumber"] = "1",
            ["searchText"] = ""
        };

        var url = QueryHelpers.AddQueryString(LobbyUrl.GetLobbies, queryParams);

        // when
        var response = await Client.GetAsync(url, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenUserWithoutAccount_WhenGetPaginatedLobbies_ThenReturnsUnauthorized()
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageSize"] = "10",
            ["pageNumber"] = "1",
            ["searchText"] = ""
        };

        var url = QueryHelpers.AddQueryString(LobbyUrl.GetLobbies, queryParams);

        // when
        var response = await Client.GetAsyncWithUser(url, TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // TODO: improve this test by parameterizing it to test multiple scenarios
    [Fact]
    public async Task GivenLobbiesAndSearchString_WhenGetPaginatedLobbies_ThenReturnsLobbiesWithMatchingNames()
    {
        // given
        await CreateUserAccount(TestUser.Default);
        await CreateUserAccount(TestUser.TestUser1);
        await CreateUserAccount(TestUser.TestUser2);

        await Client.PostAsyncWithUser(
            LobbyUrl.CreateLobby,
            TestUser.Default,
            new CreateLobbyDto
            {
                Name = "Lobby1",
                MaxPlayers = 2
            },
            TestContext.Current.CancellationToken
        );

        await Client.PostAsyncWithUser(
            LobbyUrl.CreateLobby,
            TestUser.TestUser1,
            new CreateLobbyDto
            {
                Name = "Lobby2",
                MaxPlayers = 3
            },
            TestContext.Current.CancellationToken
        );

        await Client.PostAsyncWithUser(
            LobbyUrl.CreateLobby,
            TestUser.TestUser2,
            new CreateLobbyDto
            {
                Name = "Lobby3",
                MaxPlayers = 4
            },
            TestContext.Current.CancellationToken
        );

        var queryParams = new Dictionary<string, string?>
        {
            ["pageSize"] = "10",
            ["pageNumber"] = "1",
            ["searchText"] = "2"
        };

        var url = QueryHelpers.AddQueryString(LobbyUrl.GetLobbies, queryParams);

        // when
        var response = await Client.GetAsyncWithUser(url, TestUser.Default, TestContext.Current.CancellationToken);

        var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedResult<List<LobbySummaryDto>>>(TestContext.Current.CancellationToken);

        var lobbies = paginatedResult?.Data;

        // then
        Assert.Multiple(
            () => Assert.NotNull(lobbies),
            () => Assert.Equal(1, lobbies?.Count),
            () => Assert.Equal("Lobby2", lobbies?.First().Name)
        );
    }
}