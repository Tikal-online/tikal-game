using FluentValidation.TestHelper;
using Lobbies.Application.UseCases.CreateLobby;
using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

internal sealed class CreateLobbyCommandValidatorTests
{
    // under tests
    private CreateLobbyCommandValidator validator;

    [SetUp]
    public void Setup()
    {
        validator = new CreateLobbyCommandValidator();
    }

    [TestCaseSource(
        typeof(CreateLobbyCommandTestCases),
        nameof(CreateLobbyCommandTestCases.ValidCreateLobbyCommands)
    )]
    public void GivenValidCommand_WhenValidate_ThenShouldNotHaveValidationErrors(CreateLobbyCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCaseSource(
        typeof(CreateLobbyCommandTestCases),
        nameof(CreateLobbyCommandTestCases.InvalidCreateLobbyCommands)
    )]
    public void GivenInvalidCommand_WhenValidate_ThenShouldHaveValidationErrors(CreateLobbyCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldHaveValidationErrors();
    }
}