using Moq;
using Users.Application.Usecases.Register;
using Users.Contracts.Commands;
using Users.Contracts.Dtos;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Application.Tests.Usecases.Register;

public class RegisterCommandHandlerTests
{
    #region dependencies

    private Mock<UserRepository> userRepository;

    #endregion

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
            .Setup(r => r.GetByUsername(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(User));

        // user creation succeeds
        var user = new User(1, command.Username);

        userRepository
            .Setup(r => r.CreateUser(It.IsAny<User>(), command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
    }


    [TestCaseSource(typeof(RegisterCommandTestCases), nameof(RegisterCommandTestCases.ValidRegisterCommands))]
    public async Task GivenExistingUsername_WhenHandle_ThenReturnsDuplicateUsernameError(RegisterCommand command)
    {
        // given
        SetUpHappyPath(command);

        var existingUser = new User(1, command.Username);

        userRepository
            .Setup(r => r.GetByUsername(command.Username, It.IsAny<CancellationToken>()))
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
            .Setup(r => r.CreateUser(It.IsAny<User>(), command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DuplicateUsernameError(command.Username));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<DuplicateUsernameError>());
    }

    [TestCaseSource(typeof(RegisterCommandTestCases), nameof(RegisterCommandTestCases.ValidRegisterCommands))]
    public async Task GivenUserCreationSucceeds_WhenHandle_ThenReturnsCreatedUser(RegisterCommand command)
    {
        // given
        SetUpHappyPath(command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.InstanceOf<UserDto>());
            Assert.That(result.AsT0.Username, Is.EqualTo(command.Username));
        }
    }
}