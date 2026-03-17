using System.Net;
using System.Net.Http.Json;
using Identity.IntegrationTests.Users.Register;
using NUnit.Framework;
using RestApi.Controllers.Users.Dtos;

namespace Identity.IntegrationTests.Users.Login;

internal sealed class LoginTests : IntegrationTestFixture
{
    private const string loginUrl = "login";

    private const string registerUrl = "register";

    [TestCaseSource(typeof(RegisterDtoTestCases), nameof(RegisterDtoTestCases.ValidRegisterDtos))]
    public async Task GivenExistingUser_WhenLoginWithCorrectCredentials_ThenReturnsSuccess(RegisterDto registerDto)
    {
        // given
        await Client.PostAsJsonAsync(registerUrl, registerDto);

        var loginDto = new LoginDto { Username = registerDto.Username, Password = registerDto.Password };

        // when
        var response = await Client.PostAsJsonAsync(loginUrl, loginDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(typeof(RegisterDtoTestCases), nameof(RegisterDtoTestCases.ValidRegisterDtos))]
    public async Task GivenExistingUser_WhenLoginWithIncorrectCredentials_ThenReturnsUnauthorized(
        RegisterDto registerDto
    )
    {
        // given
        await Client.PostAsJsonAsync(registerUrl, registerDto);

        var loginDto = new LoginDto { Username = registerDto.Username, Password = "wrong Password1!" };

        // when
        var response = await Client.PostAsJsonAsync(loginUrl, loginDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(LoginDtoTestCases), nameof(LoginDtoTestCases.ValidLoginDtos))]
    public async Task GivenNonExistingUser_WhenLogin_ThenReturnsUnauthorized(LoginDto loginDto)
    {
        // when
        var response = await Client.PostAsJsonAsync(loginUrl, loginDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(LoginDtoTestCases), nameof(LoginDtoTestCases.InvalidLoginDtos))]
    public async Task GivenInvalidRequest_WhenLogin_ThenReturnsBadRequest(LoginDto loginDto)
    {
        // when
        var response = await Client.PostAsJsonAsync(loginUrl, loginDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}