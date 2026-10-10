using System.Net;
using System.Net.Http.Json;
using Accounts.Contracts.Models;
using RestApi.Controllers.Accounts.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Accounts;

public sealed class GetAccountTests : IntegrationTestFixture
{
    [Fact]
    public async Task GivenUnauthenticatedUser_WhenGetAccount_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.GetAsync(AccountUrl.GetAccount, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GivenNoUserAccountForAuthenticatedUser_WhenGetAccount_ThenReturnsNotFound()
    {
        // when
        var response = await Client.GetAsyncWithUser(AccountUrl.GetAccount, TestUser.Default, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateAccountDtos))]
    public async Task GivenUserAccountForAuthenticatedUser_WhenGetAccount_ThenReturnsAccount(
        CreateAccountDto createAccountDto
    )
    {
        // given
        await Client.PostAsyncWithUser(AccountUrl.CreateAccount, TestUser.Default, createAccountDto, TestContext.Current.CancellationToken);

        // when
        var response = await Client.GetAsyncWithUser(AccountUrl.GetAccount, TestUser.Default, TestContext.Current.CancellationToken);

        var account = await response.Content.ReadFromJsonAsync<AccountModel>(TestContext.Current.CancellationToken);

        // then
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.NotNull(account),
            () => Assert.Equal(TestUser.Default.UserId, account?.UserId),
            () => Assert.Equal(createAccountDto.Name, account?.Name)
        );
    }
}