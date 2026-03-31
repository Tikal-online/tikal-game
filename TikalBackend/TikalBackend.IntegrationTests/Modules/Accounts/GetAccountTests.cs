using System.Net;
using TikalBackend.IntegrationTests.Extensions;

namespace TikalBackend.IntegrationTests.Modules.Accounts;

internal sealed class GetAccountTests : IntegrationTestFixture
{
    private const string getAccountUrl = "Accounts/me";

    [Test]
    public async Task GivenUnauthenticatedUser_WhenGetAccount_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsync(getAccountUrl);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenNoUserAccountForAuthenticatedUser_WhenGetAccount_ThenReturnsNotFound()
    {
        // when
        var response = await Client.GetAsyncWithUser(getAccountUrl, TestUser.Default);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}