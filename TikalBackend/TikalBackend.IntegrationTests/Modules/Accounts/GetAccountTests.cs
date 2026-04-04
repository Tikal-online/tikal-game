using System.Net;
using System.Net.Http.Json;
using Accounts.Contracts.Models;
using RestApi.Controllers.Accounts.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Accounts;

internal sealed class GetAccountTests : IntegrationTestFixture
{
    private const string getAccountUrl = "Accounts/me";

    private const string createAccountUrl = "Accounts";

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

    [TestCaseSource(typeof(CreateAccountDtoTestCases), nameof(CreateAccountDtoTestCases.ValidCreateAccountDtos))]
    public async Task GivenUserAccountForAuthenticatedUser_WhenGetAccount_ThenReturnsAccount(
        CreateAccountDto createAccountDto
    )
    {
        // given
        await Client.PostAsyncWithUser(createAccountUrl, TestUser.Default, createAccountDto);

        // when
        var response = await Client.GetAsyncWithUser(getAccountUrl, TestUser.Default);

        var account = await response.Content.ReadFromJsonAsync<AccountModel>();

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(account, Is.Not.Null);
            Assert.That(account!.UserId, Is.EqualTo(TestUser.Default.UserId));
            Assert.That(account.Name, Is.EqualTo(createAccountDto.Name));
        }
    }
}