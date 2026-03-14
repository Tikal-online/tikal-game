using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using Users.Contracts.Dtos;

namespace Identity.IntegrationTests.Users.Register;

public class RegisterTests : IntegrationTestFixture
{
    private const string url = "register";

    [TestCaseSource(typeof(RegisterDtoTestCases), nameof(RegisterDtoTestCases.ValidRegisterDtos))]
    public async Task GivenValidDto_WhenRegister_ThenReturnsSuccess(RegisterDto registerDto)
    {
        // when
        var response = await Client.PostAsJsonAsync(url, registerDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(typeof(RegisterDtoTestCases), nameof(RegisterDtoTestCases.InvalidRegisterDtos))]
    public async Task GivenInvalidRegisterDto_WhenRegister_ThenReturnsBadRequest(RegisterDto registerDto)
    {
        // when
        var response = await Client.PostAsJsonAsync(url, registerDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TestCaseSource(typeof(RegisterDtoTestCases), nameof(RegisterDtoTestCases.ValidRegisterDtos))]
    public async Task GivenExistingUsername_WhenRegister_ThenReturnsConflict(RegisterDto registerDto)
    {
        // given
        await Client.PostAsJsonAsync(url, registerDto);
        // when
        var response = await Client.PostAsJsonAsync(url, registerDto);

        // then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }
}