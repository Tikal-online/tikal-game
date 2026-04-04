using System.Net;
using System.Net.Http.Json;
using RestApi.Controllers.Accounts.Dtos;
using TikalBackend.IntegrationTests.Extensions;
using TikalBackend.IntegrationTests.Modules.Accounts.Dtos;

namespace TikalBackend.IntegrationTests.Modules.Accounts;

internal sealed class CreateAccountTests : IntegrationTestFixture
{
    private const string createAccountUrl = "Accounts";

    [TestCaseSource(typeof(CreateAccountDtoTestCases), nameof(CreateAccountDtoTestCases.ValidCreateAccountDtos))]
    public async Task GivenUnauthenticatedUser_WhenCreateAccount_ThenReturnsUnauthorized(
        CreateAccountDto createAccountDto
    )
    {
        // when
        var response = await Client.PostAsJsonAsync(createAccountUrl, createAccountDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(CreateAccountDtoTestCases), nameof(CreateAccountDtoTestCases.InvalidCreateAccountDtos))]
    public async Task GivenInvalidCreateAccountDto_WhenCreateAccount_ThenReturnsBadRequest(
        CreateAccountDto createAccountDto
    )
    {
        // when
        var response = await Client.PostAsyncWithUser(createAccountUrl, TestUser.Default, createAccountDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TestCaseSource(typeof(CreateAccountDtoTestCases), nameof(CreateAccountDtoTestCases.ValidCreateAccountDtos))]
    public async Task GivenExistingAccountForUser_WhenCreateAccount_ThenReturnsConflict(
        CreateAccountDto createAccountDto
    )
    {
        // given
        await Client.PostAsyncWithUser(createAccountUrl, TestUser.Default, createAccountDto);

        // when
        var response = await Client.PostAsyncWithUser(createAccountUrl, TestUser.Default, createAccountDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [TestCaseSource(typeof(CreateAccountDtoTestCases), nameof(CreateAccountDtoTestCases.ValidCreateAccountDtos))]
    public async Task GivenNoAccountForUser_WhenCreateAccount_ThenReturnsCreated(
        CreateAccountDto createAccountDto
    )
    {
        // when
        var response = await Client.PostAsyncWithUser(createAccountUrl, TestUser.Default, createAccountDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
}