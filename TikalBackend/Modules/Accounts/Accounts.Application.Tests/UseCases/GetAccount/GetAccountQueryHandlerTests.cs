using Accounts.Application.DataAccess;
using Accounts.Application.Tests.Data;
using Accounts.Application.UseCases.GetAccount;
using Accounts.Contracts.Queries;
using Accounts.Domain.Entities;
using Moq;

namespace Accounts.Application.Tests.UseCases.GetAccount;

public sealed class GetAccountQueryHandlerTests
{
    // dependencies
    private readonly Mock<AccountQueryContext> accountQueryContext;

    // under test
    private readonly GetAccountQueryHandler handler;

    public GetAccountQueryHandlerTests()
    {
        accountQueryContext = new Mock<AccountQueryContext>();

        handler = new GetAccountQueryHandler(accountQueryContext.Object);
    }

    [Theory]
    [ClassData(typeof(ValidGetAccountQueries))]
    public async Task GivenNonExistentIdentifier_WhenHandle_ThenReturnsNull(GetAccountQuery query)
    {
        // given
        accountQueryContext
            .Setup(q => q.GetByUserId(query.UserId))
            .ReturnsAsync(default(Account));

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Null(result);
    }

    [Theory]
    [ClassData(typeof(ValidAccounts))]
    public async Task GivenExistentIdentifier_WhenHandle_ThenReturnsCorrectlyMappedAccountModel(Account account)
    {
        // given
        accountQueryContext
            .Setup(q => q.GetByUserId(account.UserId))
            .ReturnsAsync(account);

        var query = new GetAccountQuery(account.UserId);

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.NotNull(result),
            () => Assert.Equal(account.Name, result?.Name),
            () => Assert.Equal(account.UserId, result?.UserId)
        );
    }
}