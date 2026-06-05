using System.Diagnostics;
using System.Security.Claims;
using Accounts.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Shared;
using Shared.Application.Contexts;

namespace TikalBackend.WebHost.Middleware;

internal sealed class AccountMiddleware
{
    private readonly RequestDelegate next;

    public AccountMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, AccountContext accountContext, ISender sender)
    {
        var endpoint = context.GetEndpoint();

        var requiresAccount = endpoint?.Metadata
            .GetMetadata<RequireAccountAttribute>() is not null;

        if (!requiresAccount)
        {
            await next(context);
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var accountModel = await sender.Send(new GetAccountQuery(userId!));

        if (accountModel is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var problemDetails = new ProblemDetails
            {
                Title = "Account not found",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "An account is required to access this functionality",
                Extensions = new Dictionary<string, object?>
                {
                    {
                        "traceId", Activity.Current?.Id ?? context.TraceIdentifier
                    }
                }
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
            return;
        }

        accountContext.Account = new UserAccount
        {
            Name = accountModel.Name,
            UserId = accountModel.UserId
        };

        await next(context);
    }
}