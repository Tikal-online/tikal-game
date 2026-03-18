using Moq;
using OneOf.Types;
using Users.Application.Usecases.Register;
using Users.Contracts.Commands;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Application.Tests.Usecases.Register;

internal sealed class RegisterCommandHandlerTests
{
    // dependencies
    private Mock<UserRepository> userRepository;

    // under test
    private RegisterCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        userRepository = new Mock<UserRepository>();

        handler = new RegisterCommandHandler(userRepository.Object);
    }

    private void SetUpHappyPath(RegisterCommand command)
    {
        // username doesnt exist
        userRepository
            .Setup(r => r.GetByUsername(command.Username))
            .ReturnsAsync(default(User));

        // user creation successful
        var user = new User { Id = 1, UserName = command.Username };

        userRepository
            .Setup(r => r.CreateUser(It.IsAny<User>(), command.Password))
            .ReturnsAsync(user);
    }

    [TestCaseSource(typeof(RegisterCommandTestCases), nameof(RegisterCommandTestCases.ValidRegisterCommands))]
    public async Task GivenExistingUsername_WhenHandle_ThenReturnsDuplicateUsernameError(RegisterCommand command)
    {
        // given
        SetUpHappyPath(command);

        var existingUser = new User { Id = 1, UserName = command.Username };

        userRepository
            .Setup(r => r.GetByUsername(command.Username))
            .ReturnsAsync(existingUser);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<DuplicateUsernameError>());
    }

    [TestCaseSource(typeof(RegisterCommandTestCases), nameof(RegisterCommandTestCases.ValidRegisterCommands))]
    public async Task GivenUserCreationFails_WhenHandle_ThenReturnsDuplicateUsernameError(RegisterCommand command)
    {
        // given
        SetUpHappyPath(command);

        userRepository
            .Setup(r => r.CreateUser(It.IsAny<User>(), command.Password))
            .ReturnsAsync(new DuplicateUsernameError(command.Username));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<DuplicateUsernameError>());
    }

    [TestCaseSource(typeof(RegisterCommandTestCases), nameof(RegisterCommandTestCases.ValidRegisterCommands))]
    public async Task GivenUserCreationSucceeds_WhenHandle_ThenReturnsSuccess(RegisterCommand command)
    {
        // given
        SetUpHappyPath(command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<Success>());
    }
}