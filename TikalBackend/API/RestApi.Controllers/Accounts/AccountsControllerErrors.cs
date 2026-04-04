using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Accounts;

public sealed partial class AccountsController
{
    private ObjectResult AccountNotFound(string userId)
    {
        return Problem(
            title: "Account not found",
            detail: $"Account with userId '{userId}' was not found",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private ObjectResult AccountAlreadyExists(string userId)
    {
        return Problem(
            title: "Account already exists",
            detail: $"Account with userId '{userId}' already exists",
            statusCode: StatusCodes.Status409Conflict
        );
    }
}