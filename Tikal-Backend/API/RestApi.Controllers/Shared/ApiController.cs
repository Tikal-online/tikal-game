using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Shared;

[Authorize]
[ApiController]
[Route("[controller]")]
public abstract class ApiController : ControllerBase
{
    protected ObjectResult MissingRequiredClaim(string claimName)
    {
        return Problem(
            title: "Missing required token claim",
            detail:
            $"The token is missing the required claim '{claimName}'. This indicates a misconfiguration in the identity server",
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
}