using System.Net;
using System.Net.Http.Json;
using Identity.IntegrationTests.Users.Register;
using NUnit.Framework;
using RestApi.Controllers.Users.Dtos;

namespace Identity.IntegrationTests.Users.Refresh;

internal sealed class RefreshTests : IntegrationTestFixture
{
    private const string registerUrl = "register";

    private const string loginUrl = "login";

    private const string refreshUrl = "refresh";

    private async Task<string> RegisterUserAndGetRefreshToken(RegisterDto registerDto)
    {
        await Client.PostAsJsonAsync(registerUrl, registerDto);

        var loginDto = new LoginDto { Username = registerDto.Username, Password = registerDto.Password };

        var response = await Client.PostAsJsonAsync(loginUrl, loginDto);

        var cookies = response.Headers.GetValues("Set-Cookie");

        foreach (var cookie in cookies)
        {
            var parts = cookie.Split('=');

            if (parts[0] == "refresh_token")
            {
                return parts[1];
            }
        }

        return string.Empty;
    }

    [Test]
    public async Task GivenNoRefreshToken_WhenRefresh_ThenReturnsUnauthorized()
    {
        // when
        var response = await Client.PostAsync(refreshUrl, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(RefreshTokenTestCases), nameof(RefreshTokenTestCases.InvalidRefreshTokens))]
    public async Task GivenInvalidRefreshToken_WhenRefresh_ThenReturnsUnauthorized(string invalidRefreshToken)
    {
        // given
        Client.DefaultRequestHeaders.Add("Cookie", $"refresh_token={invalidRefreshToken}");

        // when
        var response = await Client.PostAsync(refreshUrl, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [TestCaseSource(typeof(RegisterDtoTestCases), nameof(RegisterDtoTestCases.ValidRegisterDtos))]
    public async Task GivenValidRefreshToken_WhenRefresh_ThenReturnsSuccess(RegisterDto registerDto)
    {
        // given
        var refreshToken = await RegisterUserAndGetRefreshToken(registerDto);

        Client.DefaultRequestHeaders.Add("Cookie", $"refresh_token={refreshToken}");

        // when
        var response = await Client.PostAsync(refreshUrl, null);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}