using Accounts.Application.UseCases.CreateAccount;
using Accounts.Contracts.Commands;
using FluentValidation.TestHelper;

namespace Accounts.Application.Tests.UseCases.CreateAccount;

public sealed class CreateAccountCommandValidatorTests
{
    // under test
    private readonly CreateAccountCommandValidator validator;

    public CreateAccountCommandValidatorTests()
    {
        validator = new CreateAccountCommandValidator();
    }

    [Theory]
    [ClassData(typeof(ValidCreateAccountCommands))]
    public void GivenValidCommand_WhenValidate_ThenShouldNotHaveValidationErrors(CreateAccountCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [ClassData(typeof(InvalidCreateAccountCommands))]
    public void GivenInvalidCommand_WhenValidate_ThenShouldHaveValidationErrors(CreateAccountCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldHaveValidationErrors();
    }
}