using Accounts.Contracts.Commands;

namespace Accounts.Application.Tests.UseCases.CreateAccount;

internal class ValidCreateAccountCommands : TheoryData<CreateAccountCommand>
{
    public ValidCreateAccountCommands()
    {
        Add(new CreateAccountCommand("8d99e08c-3bd7-4dbc-a011-017158e89e11", "AccountName"));
        Add(new CreateAccountCommand("46ba4cab-8fe1-453f-8a4a-3a4b4ae021e9", ".H7Z>vIfwZ8<zSl;05S{WF:/MVd|rr"));
        Add(new CreateAccountCommand("1664586a-997a-4394-b139-f58fc716805a", "a    b    c"));
    }
}