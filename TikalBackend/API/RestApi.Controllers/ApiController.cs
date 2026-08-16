using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public abstract class ApiController : ControllerBase
{
    protected string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }

    protected ObjectResult Unprivileged()
    {
        return Problem(
            title: "You are not allowed to perform this action",
            detail: "You are missing requirements to perform this actions",
            statusCode: StatusCodes.Status403Forbidden
        );
    }
}