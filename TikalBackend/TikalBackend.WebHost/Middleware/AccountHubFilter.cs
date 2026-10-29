using System.Security.Claims;
using Accounts.Contracts.Models;
using Accounts.Contracts.Queries;
using Microsoft.AspNetCore.SignalR;
using Shared.Application.Contexts;
using Wolverine;

namespace TikalBackend.WebHost.Middleware;

internal sealed class AccountHubFilter : IHubFilter
{
    private readonly AccountContext accountContext;

    private readonly IMessageBus bus;

    public AccountHubFilter(AccountContext accountContext, IMessageBus bus)
    {
        this.accountContext = accountContext;
        this.bus = bus;
    }

    public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        var userId = context.Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            throw new HubException("Could not read userid from claims");
        }

        var accountModel = await bus.InvokeAsync<AccountModel?>(new GetAccountQuery(userId));

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

    public ValueTask<object?> InvokeMethodAsync(
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

        return next(invocationContext);
    }
}