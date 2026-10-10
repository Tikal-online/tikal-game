using FluentValidation.TestHelper;
using Lobbies.Application.UseCases.CreateLobby;
using Lobbies.Contracts.Commands;

namespace Lobbies.Application.Tests.UseCases.CreateLobby;

public sealed class CreateLobbyCommandValidatorTests
{
    // under tests
    private readonly CreateLobbyCommandValidator validator;

    public CreateLobbyCommandValidatorTests()
    {
        validator = new CreateLobbyCommandValidator();
    }

    [Theory]
    [ClassData(typeof(ValidCreateLobbyCommands))]
    public void GivenValidCommand_WhenValidate_ThenShouldNotHaveValidationErrors(CreateLobbyCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [ClassData(typeof(InvalidCreateLobbyCommands))]
    public void GivenInvalidCommand_WhenValidate_ThenShouldHaveValidationErrors(CreateLobbyCommand command)
    {
        // when
        var result = validator.TestValidate(command);

        // then
        result.ShouldHaveValidationErrors();
    }
}