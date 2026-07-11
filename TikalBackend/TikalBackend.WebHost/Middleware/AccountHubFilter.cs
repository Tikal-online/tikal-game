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

    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        var userId = context.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            throw new HubException("Could not read userid from claims");
        }

        var accountModel = await sender.Send(new GetAccountQuery(userId));

        if (accountModel is null)
        {
            throw new HubException("Account required");
        }

        var account = new UserAccount
        {
            Name = accountModel.Name,
            UserId = accountModel.UserId
        };

        context.Context.Items["Account"] = account;

        accountContext.Account = account;

        await next(context);
    }

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next
    )
    {
        if (invocationContext.Context.Items.TryGetValue("Account", out var value) && value is UserAccount account)
        {
            accountContext.Account = account;
        }
        else
        {
            throw new HubException("Account required");
        }

        return await next(invocationContext);
    }
}