using Accounts.Contracts.Queries;

namespace Accounts.Application.Tests.UseCases.GetAccount;

internal class ValidGetAccountQueries : TheoryData<GetAccountQuery>
{
    public ValidGetAccountQueries()
    {
        Add(new GetAccountQuery("userId"));
        Add(new GetAccountQuery("ee7e420c-c1d2-4f8e-a995-fb98b603e7ee"));
        Add(new GetAccountQuery("4a4dbd64-ab72-4757-b2de-de2f32403d2b"));
    }
}