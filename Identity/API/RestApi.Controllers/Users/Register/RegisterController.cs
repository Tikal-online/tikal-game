using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Contracts.Commands;
using Users.Contracts.Dtos;
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
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(registerDto.Username, registerDto.Password);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            handleSuccess,
            handleDuplicateUsernameError
        );
    }

    private OkObjectResult handleSuccess(
        UserDto userDto
    )
    {
        return Ok(userDto);
    }

    private IActionResult handleDuplicateUsernameError(DuplicateUsernameError duplicateUsernameError)
    {
        return UsernameExists(duplicateUsernameError.Username);
    }
}