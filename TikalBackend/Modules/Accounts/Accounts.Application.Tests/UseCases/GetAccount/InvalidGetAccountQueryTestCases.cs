using Accounts.Contracts.Queries;

namespace Accounts.Application.Tests.UseCases.GetAccount;

internal class InvalidGetAccountQueries : TheoryData<GetAccountQuery>
{
    public InvalidGetAccountQueries()
    {
        // empty user id
        Add(new GetAccountQuery(""));
        // user id longer than 100 characters
        Add(new GetAccountQuery("Q/:~$%c_<QMg`Z?4QfYo1XtM2qUB!IUZ0B:|#@!<RBE4ZDStFI=O=[d1-0&W2#Y$N*~AK|i0%/UT%M2x4EmdF$Q}3!I,$|jC}3;b0"));
    }
}