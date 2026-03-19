using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Users.Logout;

[ApiController]
[Route("[controller]")]
public sealed class LogoutController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointDescription("Removes all active cookies")]
    public IActionResult Logout()
    {
        foreach (var cookie in Request.Cookies)
        {
            Response.Cookies.Delete(cookie.Key);
        }

        return Ok();
    }
}