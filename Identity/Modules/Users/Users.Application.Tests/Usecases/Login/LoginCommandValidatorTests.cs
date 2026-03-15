using FluentValidation.TestHelper;
using Users.Application.Usecases.Login;
using Users.Contracts.Commands;

namespace Users.Application.Tests.Usecases.Login;

internal sealed class LoginCommandValidatorTests
{
    // under test
    private LoginCommandValidator validator;

    [SetUp]
    public void Setup()
    {
        validator = new LoginCommandValidator();
    }

    [TestCaseSource(typeof(LoginCommandTestCases), nameof(LoginCommandTestCases.ValidLoginCommands))]
    public void GivenValidCommand_WhenValidate_ThenShouldNotHaveValidationErrors(LoginCommand validCommand)
    {
        // when
        var result = validator.TestValidate(validCommand);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCaseSource(typeof(LoginCommandTestCases), nameof(LoginCommandTestCases.InvalidLoginCommands))]
    public void GivenInvalidCommand_WhenValidate_ThenShouldHaveValidationErrors(LoginCommand invalidCommand)
    {
        // when
        var result = validator.TestValidate(invalidCommand);

        // then
        result.ShouldHaveValidationErrors();
    }
}