using Accounts.Application.UseCases.GetAccount;
using Accounts.Contracts.Queries;
using FluentValidation.TestHelper;

namespace Accounts.Application.Tests.UseCases.GetAccount;

public sealed class GetAccountQueryValidatorTests
{
    // under test
    private readonly GetAccountQueryValidator validator;

    public GetAccountQueryValidatorTests()
    {
        validator = new GetAccountQueryValidator();
    }

    [Theory]
    [ClassData(typeof(ValidGetAccountQueries))]
    public void GivenValidQuery_WhenValidate_ThenShouldNotHaveValidationErrors(GetAccountQuery query)
    {
        // when
        var result = validator.TestValidate(query);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [ClassData(typeof(InvalidGetAccountQueries))]
    public void GivenInvalidQuery_WhenValidate_ThenShouldHaveValidationErrors(GetAccountQuery query)
    {
        // when
        var result = validator.TestValidate(query);

        // then
        result.ShouldHaveValidationErrors();
    }
}