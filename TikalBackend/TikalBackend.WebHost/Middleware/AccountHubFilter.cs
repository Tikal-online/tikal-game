using System.Security.Claims;
using Accounts.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Shared.Application.Contexts;

namespace TikalBackend.WebHost.Middleware;

internal sealed class AccountHubFilter : IHubFilter
{
    private readonly AccountContext accountContext;

    private readonly ISender sender;

    public AccountHubFilter(AccountContext accountContext, ISender sender)
    {
        this.accountContext = accountContext;
        this.sender = sender;
    }

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next
    )
    {
        var userId = invocationContext.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            throw new HubException("Could not read userid from claims");
        }

        var accountModel = await sender.Send(new GetAccountQuery(userId));

        if (accountModel is null)
        {
            throw new HubException("Account required");
        }

        accountContext.Account = new UserAccount
        {
            Name = accountModel.Name,
            UserId = accountModel.UserId
        };

        return await next(invocationContext);
    }
}