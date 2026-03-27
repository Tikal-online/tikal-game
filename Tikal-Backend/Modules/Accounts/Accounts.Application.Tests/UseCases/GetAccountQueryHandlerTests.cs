using Accounts.Application.DataAccess;
using Accounts.Application.Tests.Data;
using Accounts.Application.UseCases.GetAccount;
using Accounts.Contracts.Queries;
using Accounts.Domain.Entities;
using Moq;

namespace Accounts.Application.Tests.UseCases;

internal sealed class GetAccountQueryHandlerTests
{
    // dependencies
    private Mock<AccountQueryContext> accountQueryContext;

    // under test
    private GetAccountQueryHandler handler;

    [SetUp]
    public void Setup()
    {
        accountQueryContext = new Mock<AccountQueryContext>();

        handler = new GetAccountQueryHandler(accountQueryContext.Object);
    }

    [TestCaseSource(typeof(GetAccountQueryTestCases), nameof(GetAccountQueryTestCases.ValidGetAccountQueries))]
    public async Task GivenNonExistentIdentifier_WhenHandle_ThenReturnsNull(GetAccountQuery query)
    {
        // given
        accountQueryContext
            .Setup(q => q.GetByUserIdAsync(query.UserId))
            .ReturnsAsync(default(Account));

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        Assert.That(result, Is.Null);
    }

    [TestCaseSource(typeof(AccountTestCases), nameof(AccountTestCases.ValidAccountTestCases))]
    public async Task GivenExistentIdentifier_WhenHandle_ThenReturnsCorrectlyMappedAccountModel(Account account)
    {
        // given
        accountQueryContext
            .Setup(q => q.GetByUserIdAsync(account.UserId))
            .ReturnsAsync(account);

        var query = new GetAccountQuery(account.UserId);

        // when
        var result = await handler.Handle(query, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(account.Name));
            Assert.That(result.UserId, Is.EqualTo(account.UserId));
        }
    }
}