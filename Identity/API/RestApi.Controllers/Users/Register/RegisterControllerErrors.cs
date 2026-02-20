using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Users.Register;

public partial class RegisterController
{
    private ObjectResult UsernameExists(string username)
    {
        return Problem(
            title: "Username already exists",
            detail: $"A User with the username '{username}' already exists'",
            statusCode: StatusCodes.Status409Conflict
        );
    }
}