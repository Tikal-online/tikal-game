using Accounts.Contracts.Queries;

namespace Accounts.Application.Tests.UseCases;

internal static class GetAccountQueryTestCases
{
    public static IEnumerable<GetAccountQuery> InvalidGetAccountQueries =>
    [
        // empty user id
        new(""),
        // user id longer than 100 characters
        new("Q/:~$%c_<QMg`Z?4QfYo1XtM2qUB!IUZ0B:|#@!<RBE4ZDStFI=O=[d1-0&W2#Y$N*~AK|i0%/UT%M2x4EmdF$Q}3!I,$|jC}3;b0")
    ];

    public static IEnumerable<GetAccountQuery> ValidGetAccountQueries =>
    [
        new("userId"),
        new("ee7e420c-c1d2-4f8e-a995-fb98b603e7ee"),
        new("4a4dbd64-ab72-4757-b2de-de2f32403d2b")
    ];
}