using Accounts.Application.UseCases.GetAccount;
using Accounts.Contracts.Queries;
using FluentValidation.TestHelper;

namespace Accounts.Application.Tests.UseCases.GetAccount;

internal sealed class GetAccountQueryValidatorTests
{
    // under test
    private GetAccountQueryValidator validator;

    [SetUp]
    public void Setup()
    {
        validator = new GetAccountQueryValidator();
    }

    [TestCaseSource(typeof(GetAccountQueryTestCases), nameof(GetAccountQueryTestCases.ValidGetAccountQueries))]
    public void GivenValidQuery_WhenValidate_ThenShouldNotHaveValidationErrors(GetAccountQuery query)
    {
        // when
        var result = validator.TestValidate(query);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCaseSource(typeof(GetAccountQueryTestCases), nameof(GetAccountQueryTestCases.InvalidGetAccountQueries))]
    public void GivenInvalidQuery_WhenValidate_ThenShouldHaveValidationErrors(GetAccountQuery query)
    {
        // when
        var result = validator.TestValidate(query);

        // then
        result.ShouldHaveValidationErrors();
    }
}