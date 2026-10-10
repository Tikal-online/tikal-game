using Accounts.Contracts.Commands;

namespace Accounts.Application.Tests.UseCases.CreateAccount;

internal class InvalidCreateAccountCommands : TheoryData<CreateAccountCommand>
{
    public InvalidCreateAccountCommands()
    {
        // empty user id and name
        Add(new CreateAccountCommand("", ""));
        // empty user id
        Add(new CreateAccountCommand("", "AccountName123$!"));
        // empty name
        Add(new CreateAccountCommand("5e47c611-a224-4624-b2a3-226d835f6077", "  "));
        // user id longer than 100 characters and name longer than 30 characters
        Add(new CreateAccountCommand("/rHpaoa!>zt%pC!SN!nW!D9&0%?^Ok_R8|7~!g/PsvX3(sDT829Fp%sqm<|Re!6$glQueGLiNJ2TCPsJ4CnHeX7=Gx<qnsTA>K8v]", ".FgT+s2}j^q@qgf5b=y&*D0:*YGf,Qj"));
        // user id longer than 100 characters
        Add(new CreateAccountCommand("_#|N:f3cWWWVF$fJYUG7oKL>R!%Qbjx~qHdM]CWT%}(/!I@^d-^0X8p->/=aA`dz-|^h1C?]uI,~K=ZW%Ocy|gdR/|-x[q)_2]eej", "MyNewAccountXOXOXO"));
        // account name longer than 30 characters
        Add(new CreateAccountCommand("c14ce26d-e835-4b67-82da-ce27a5550374", "FF5v`mipBggCCR6K)ud>Nr?UIzu*cC_"));
    }
}