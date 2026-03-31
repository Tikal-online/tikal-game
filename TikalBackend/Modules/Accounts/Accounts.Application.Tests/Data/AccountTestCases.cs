using Accounts.Domain.Entities;

namespace Accounts.Application.Tests.Data;

internal static class AccountTestCases
{
    public static IEnumerable<Account> ValidAccountTestCases =>
    [
        new() { Name = "AccountName", UserId = "UserId" },
        new() { Name = "1", UserId = "2" },
        new() { Name = "TestUser123!$$", UserId = "871d9634-843b-4dd3-85c4-9806e8968a1f" },
        new() { Name = "PRus4~pL9>6uCm][T;G7", UserId = "(5uKc[[SB@>NCPKXL@00]7m35GB~Z+;M9L9H$hvQN2$ltmDC;^AT-gyuhm3!" }
    ];
}