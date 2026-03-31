using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Shared;

[Authorize]
[ApiController]
[Route("[controller]")]
public abstract class ApiController : ControllerBase
{
    protected string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}