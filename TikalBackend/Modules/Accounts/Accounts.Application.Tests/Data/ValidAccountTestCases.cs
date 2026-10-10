using Accounts.Domain.Entities;

namespace Accounts.Application.Tests.Data;

internal class ValidAccounts : TheoryData<Account>
{
    public ValidAccounts()
    {
        Add(new Account() { Name = "AccountName", UserId = "UserId" });
        Add(new Account() { Name = "1", UserId = "2" });
        Add(new Account() { Name = "TestUser123!$$", UserId = "871d9634-843b-4dd3-85c4-9806e8968a1f" });
        Add(new Account() { Name = "PRus4~pL9>6uCm][T;G7", UserId = "(5uKc[[SB@>NCPKXL@00]7m35GB~Z+;M9L9H$hvQN2$ltmDC;^AT-gyuhm3!" });
    }
}