using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Users.Dtos;
using Users.Contracts.Commands;
using Users.Contracts.Errors;

namespace RestApi.Controllers.Users.Register;

[ApiController]
[Route("[controller]")]
public partial class RegisterController : ControllerBase
{
    private readonly ISender sender;

    public RegisterController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointDescription("Registers a new user with the given credentials")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(registerDto.Username, registerDto.Password);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(),
            handleDuplicateUsernameError
        );
    }

    private IActionResult handleDuplicateUsernameError(DuplicateUsernameError duplicateUsernameError)
    {
        return UsernameExists(duplicateUsernameError.Username);
    }
}