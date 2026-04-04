using Accounts.Contracts.Commands;

namespace Accounts.Application.Tests.UseCases.CreateAccount;

internal static class CreateAccountCommandTestCases
{
    public static IEnumerable<CreateAccountCommand> InvalidCreateAccountCommands =>
    [
        // empty user id and name
        new("", ""),
        // empty user id
        new("     ", "AccountName123$!"),
        // empty name
        new("5e47c611-a224-4624-b2a3-226d835f6077", "  "),
        // user id longer than 100 characters and name longer than 30 characters
        new("/rHpaoa!>zt%pC!SN!nW!D9&0%?^Ok_R8|7~!g/PsvX3(sDT829Fp%sqm<|Re!6$glQueGLiNJ2TCPsJ4CnHeX7=Gx<qnsTA>K8v]",
            ".FgT+s2}j^q@qgf5b=y&*D0:*YGf,Qj"),
        // user id longer than 100 characters
        new("_#|N:f3cWWWVF$fJYUG7oKL>R!%Qbjx~qHdM]CWT%}(/!I@^d-^0X8p->/=aA`dz-|^h1C?]uI,~K=ZW%Ocy|gdR/|-x[q)_2]eej",
            "MyNewAccountXOXOXO"),
        // account name longer than 30 characters
        new("c14ce26d-e835-4b67-82da-ce27a5550374", "FF5v`mipBggCCR6K)ud>Nr?UIzu*cC_")
    ];

    public static IEnumerable<CreateAccountCommand> ValidCreateAccountCommands =>
    [
        new("8d99e08c-3bd7-4dbc-a011-017158e89e11", "AccountName"),
        new("46ba4cab-8fe1-453f-8a4a-3a4b4ae021e9", ".H7Z>vIfwZ8<zSl;05S{WF:/MVd|rr"),
        new("1664586a-997a-4394-b139-f58fc716805a", "a    b    c")
    ];
}