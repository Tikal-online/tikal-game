using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Extensions;
using RestApi.Controllers.Users.Dtos;
using Users.Contracts.Commands;
using Users.Contracts.Models;

namespace RestApi.Controllers.Users.Login;

[ApiController]
[Route("[controller]")]
public sealed partial class LoginController : ControllerBase
{
    private readonly ISender sender;

    public LoginController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    [ProducesResponseType<TokenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointDescription("Login with a pair or credentials")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(loginDto.Username, loginDto.Password);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            handleSuccess,
            _ => handleInvalidCredentials()
        );
    }

    private OkObjectResult handleSuccess(TokenPair tokenPair)
    {
        Response.Cookies.AddRefreshToken(tokenPair.RefreshToken);

        var tokenDto = new TokenDto { AccessToken = tokenPair.AccessToken };

        return Ok(tokenDto);
    }

    private ObjectResult handleInvalidCredentials()
    {
        return InvalidCredentials();
    }
}