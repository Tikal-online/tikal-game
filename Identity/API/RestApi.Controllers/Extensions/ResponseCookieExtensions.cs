using Microsoft.AspNetCore.Http;

namespace RestApi.Controllers.Extensions;

internal static class ResponseCookieExtensions
{
    extension(IResponseCookies cookies)
    {
        public void AddRefreshToken(string refreshToken)
        {
            cookies.Append(
                "refresh_token",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                }
            );
        }
    }
}