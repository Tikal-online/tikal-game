using Accounts.Application.UseCases.CreateAccount;
using Accounts.Contracts.Commands;
using FluentValidation.TestHelper;

namespace Accounts.Application.Tests.UseCases.CreateAccount;

internal sealed class CreateAccountCommandValidatorTests
{
    // under test
    private CreateAccountCommandValidator validator;

    [SetUp]
    public void Setup()
    {
        validator = new CreateAccountCommandValidator();
    }

    [TestCaseSource(
        typeof(CreateAccountCommandTestCases),
        nameof(CreateAccountCommandTestCases.ValidCreateAccountCommands)
    )]
    public void GivenValidCommand_WhenValidate_ThenShouldNotHaveValidationErrors(CreateAccountCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCaseSource(
        typeof(CreateAccountCommandTestCases),
        nameof(CreateAccountCommandTestCases.InvalidCreateAccountCommands)
    )]
    public void GivenInvalidCommand_WhenValidate_ThenShouldHaveValidationErrors(CreateAccountCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldHaveValidationErrors();
    }
}