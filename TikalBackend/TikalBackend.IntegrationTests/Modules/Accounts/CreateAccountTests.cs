using System.Net;
using System.Net.Http.Json;
using Accounts.Contracts.Models;
using RestApi.Controllers.Accounts.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Accounts;

public sealed class CreateAccountTests : IntegrationTestFixture
{
    [Theory]
    [ClassData(typeof(ValidCreateAccountDtos))]
    public async Task GivenUnauthenticatedUser_WhenCreateAccount_ThenReturnsUnauthorized(
        CreateAccountDto createAccountDto
    )
    {
        // when
        var response = await Client.PostAsJsonAsync(AccountUrl.CreateAccount, createAccountDto, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(InvalidCreateAccountDtos))]
    public async Task GivenInvalidCreateAccountDto_WhenCreateAccount_ThenReturnsBadRequest(
        CreateAccountDto createAccountDto
    )
    {
        // when
        var response = await Client.PostAsyncWithUser(AccountUrl.CreateAccount, TestUser.Default, createAccountDto);

        // then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateAccountDtos))]
    public async Task GivenExistingAccountForUser_WhenCreateAccount_ThenReturnsConflict(
        CreateAccountDto createAccountDto
    )
    {
        // given
        await Client.PostAsyncWithUser(AccountUrl.CreateAccount, TestUser.Default, createAccountDto);

        // when
        var response = await Client.PostAsyncWithUser(AccountUrl.CreateAccount, TestUser.Default, createAccountDto);

        // then
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(ValidCreateAccountDtos))]
    public async Task GivenNoAccountForUser_WhenCreateAccount_ThenReturnsCreatedAccount(
        CreateAccountDto createAccountDto
    )
    {
        // when
        var response = await Client.PostAsyncWithUser(AccountUrl.CreateAccount, TestUser.Default, createAccountDto);

        var account = await response.Content.ReadFromJsonAsync<AccountModel>(TestContext.Current.CancellationToken);

        // then
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode),
            () => Assert.NotNull(account),
            () => Assert.Equal(TestUser.Default.UserId, account?.UserId),
            () => Assert.Equal(createAccountDto.Name, account?.Name)
        );
    }
}