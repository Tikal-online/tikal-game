using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Users.Refresh;

public sealed partial class RefreshController
{
    private ObjectResult InvalidRefreshToken()
    {
        return Problem(
            title: "Invalid refresh token",
            statusCode: StatusCodes.Status401Unauthorized
        );
    }
}