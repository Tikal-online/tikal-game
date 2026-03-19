using NUnit.Framework;

namespace Identity.IntegrationTests.Users.Logout;

internal sealed class LogoutTests : IntegrationTestFixture
{
    private const string logoutUrl = "logout";

    [TestCase("test_cookie=test")]
    [TestCase("test_cookie_1=test", "test_cookie_2=test", "test_cookie_3=test")]
    public async Task GivenCookies_WhenLogout_ThenExpiresAllCookies(params string[] cookies)
    {
        // given
        Client.DefaultRequestHeaders.Add("Cookie", string.Join(";", cookies));

        // when
        var response = await Client.PostAsync(logoutUrl, null);

        var cookieHeaders = response.Headers.GetValues("Set-Cookie").ToList();

        // then
        foreach (var cookieHeader in cookieHeaders)
        {
            var parts = cookieHeader.Split(';');

            DateTime? expires = null;

            foreach (var part in parts)
            {
                if (part.StartsWith(" expires="))
                {
                    expires = DateTime.Parse(part[9..]);
                }
            }

            Assert.That(expires, Is.LessThan(DateTime.Now));
        }

        Assert.That(cookieHeaders, Has.Count.EqualTo(cookies.Length));
    }
}