using Shared.Application.Contexts;

namespace Shared.Application.Tests;

public static class AccountContextHelper
{
    public static AccountContext TestAccountContext =>
        new()
        {
            Account = new UserAccount
            {
                Name = "TestAccount",
                UserId = "8761421d-cb45-41ca-8aba-350457624ea9"
            }
        };
}