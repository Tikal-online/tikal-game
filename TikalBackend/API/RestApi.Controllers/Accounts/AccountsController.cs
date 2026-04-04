using Accounts.Contracts.Commands;
using Accounts.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Accounts.Dtos;
using RestApi.Controllers.Shared;

namespace RestApi.Controllers.Accounts;

public sealed partial class AccountsController : ApiController
{
    private readonly ISender sender;

    public AccountsController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet("me")]
    [ProducesResponseType<AccountDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Gets the account for the currently authenticated user")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var query = new GetAccountQuery(userId);

        var result = await sender.Send(query, cancellationToken);

        if (result is null)
        {
            return AccountNotFound(userId);
        }

        var dto = new AccountDto { Name = result.Name, UserId = result.UserId };

        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointDescription("Creates a new account for the currently authenticated user")]
    public async Task<IActionResult> CreateAccount(
        CreateAccountDto createAccountDto,
        CancellationToken cancellationToken
    )
    {
        var userId = GetCurrentUserId();

        var command = new CreateAccountCommand(userId, createAccountDto.Name);

        var result = await sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => CreatedAtAction(nameof(GetMe), null),
            duplicateUserId => AccountAlreadyExists(duplicateUserId.UserId)
        );
    }
}