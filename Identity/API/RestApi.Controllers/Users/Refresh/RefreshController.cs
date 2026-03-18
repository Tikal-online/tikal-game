using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Extensions;
using RestApi.Controllers.Users.Dtos;
using Users.Contracts.Commands;
using Users.Contracts.Models;

namespace RestApi.Controllers.Users.Refresh;

[ApiController]
[Route("[controller]")]
public sealed partial class RefreshController : ControllerBase
{
    private readonly ISender sender;

    public RefreshController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointDescription("Uses refresh token to acquire a new pair of tokens")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return InvalidRefreshToken();
        }

        var command = new RefreshCommand(refreshToken);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            handleSuccess,
            _ => handleInvalidToken()
        );
    }

    private OkObjectResult handleSuccess(TokenPair tokenPair)
    {
        Response.Cookies.AddRefreshToken(tokenPair.RefreshToken);

        var tokenDto = new TokenDto { AccessToken = tokenPair.AccessToken };

        return Ok(tokenDto);
    }

    private ObjectResult handleInvalidToken()
    {
        return InvalidRefreshToken();
    }
}