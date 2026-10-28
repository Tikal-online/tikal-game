using Accounts.Contracts.Commands;
using Accounts.Contracts.Errors;
using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using RestApi.Controllers.Accounts.Dtos;
using Wolverine;

namespace RestApi.Controllers.Accounts;

public sealed partial class AccountsController : ApiController
{
    private readonly IMessageBus bus;

    public AccountsController(IMessageBus bus)
    {
        this.bus = bus;
    }

    [HttpGet("me")]
    [ProducesResponseType<AccountDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Gets the account for the currently authenticated user")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var query = new GetAccountQuery(userId);

        var result = await bus.InvokeAsync<AccountModel?>(query, cancellationToken);

        if (result is null)
        {
            return AccountNotFound(userId);
        }

        var dto = AccountDto.FromModel(result);

        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType<AccountDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointDescription("Creates a new account for the currently authenticated user")]
    public async Task<IActionResult> CreateAccount(
        CreateAccountDto createAccountDto,
        CancellationToken cancellationToken
    )
    {
        var userId = GetCurrentUserId();

        var command = new CreateAccountCommand(userId, createAccountDto.Name);

        var result = await bus.InvokeAsync<OneOf<AccountModel, DuplicateUserId>>(command, cancellationToken);

        return result.Match<IActionResult>(
            accountModel => CreatedAtAction(nameof(GetMe), AccountDto.FromModel(accountModel)),
            duplicateUserId => AccountAlreadyExists(duplicateUserId.UserId)
        );
    }
}