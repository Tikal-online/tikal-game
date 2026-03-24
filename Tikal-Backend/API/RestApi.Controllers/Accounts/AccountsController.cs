using Accounts.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Accounts.Dtos;

namespace RestApi.Controllers.Accounts;

[ApiController]
[Route("[controller]")]
public sealed partial class AccountsController : ControllerBase
{
    private readonly ISender sender;

    public AccountsController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet("/me")]
    [ProducesResponseType<AccountDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Gets the account for the currently authenticated user")]
    [Authorize]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var query = new GetAccountQuery("myUserId");

        var result = await sender.Send(query, cancellationToken);

        if (result is null)
        {
            return AccountNotFound("myUserId");
        }

        var dto = new AccountDto { Name = result.Name, UserId = result.UserId };

        return Ok(dto);
    }
}