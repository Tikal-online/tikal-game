using FluentValidation.TestHelper;
using Users.Application.Usecases.Register;
using Users.Contracts.Commands;

namespace Users.Application.Tests.Usecases.Register;

public class RegisterCommandValidatorTests
{
    // under test
    private RegisterCommandValidator validator;

    [SetUp]
    public void Setup()
    {
        validator = new RegisterCommandValidator();
    }

    [TestCaseSource(typeof(RegisterCommandTestCases), nameof(RegisterCommandTestCases.ValidRegisterCommands))]
    public void GivenValidCommand_WhenValidate_ThenShouldNotHaveValidationErrors(RegisterCommand validCommand)
    {
        // when
        var result = validator.TestValidate(validCommand);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCaseSource(typeof(RegisterCommandTestCases), nameof(RegisterCommandTestCases.InvalidRegisterCommands))]
    public void GivenInvalidCommand_WhenValidate_ThenShouldHaveValidationErrors(RegisterCommand invalidCommand)
    {
        // when
        var result = validator.TestValidate(invalidCommand);

        // then
        result.ShouldHaveValidationErrors();
    }
}