using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Users.Login;

public sealed partial class LoginController
{
    private ObjectResult InvalidCredentials()
    {
        return Problem(
            title: "Invalid credentials",
            detail: "Invalid username or password provided",
            statusCode: StatusCodes.Status401Unauthorized
        );
    }
}