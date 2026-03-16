using Moq;
using Users.Application.Services;
using Users.Application.Usecases.Login;
using Users.Contracts.Commands;
using Users.Contracts.Errors;
using Users.Contracts.Models;
using Users.Domain.Entities;

namespace Users.Application.Tests.Usecases.Login;

internal sealed class LoginCommandHandlerTests
{
    private const string AccessToken = "accessToken";
    private const string RefreshToken = "refreshToken";

    // dependencies
    private Mock<UserRepository> userRepository;
    private Mock<TokenService> tokenService;

    // under test
    private LoginCommandHandler handler;

    [SetUp]
    public void SetUp()
    {
        userRepository = new Mock<UserRepository>();
        tokenService = new Mock<TokenService>();

        handler = new LoginCommandHandler(userRepository.Object, tokenService.Object);
    }

    private void SetUpHappyPath(LoginCommand command)
    {
        var user = new User { Id = 1, UserName = command.Username };

        // user with username exists
        userRepository
            .Setup(r => r.GetByUsername(command.Username))
            .ReturnsAsync(user);

        // provided password is valid
        userRepository
            .Setup(r => r.ValidatePassword(user, command.Password))
            .ReturnsAsync(true);

        // tokens are generated
        List<string> roles = ["User"];

        userRepository
            .Setup(r => r.GetRoles(user))
            .ReturnsAsync(roles);

        tokenService
            .Setup(s => s.GenerateAccessToken(user, roles))
            .Returns(AccessToken);

        tokenService
            .Setup(s => s.GenerateRefreshToken(user, roles))
            .Returns(RefreshToken);
    }

    [TestCaseSource(typeof(LoginCommandTestCases), nameof(LoginCommandTestCases.ValidLoginCommands))]
    public async Task GivenNonExistingUsername_WhenHandle_ThenReturnsInvalidCredentialsError(LoginCommand command)
    {
        // given
        SetUpHappyPath(command);

        userRepository
            .Setup(r => r.GetByUsername(command.Username))
            .ReturnsAsync(default(User));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<InvalidCredentialsError>());
    }

    [TestCaseSource(typeof(LoginCommandTestCases), nameof(LoginCommandTestCases.ValidLoginCommands))]
    public async Task GivenInvalidPassword_WhenHandle_ThenReturnsInvalidCredentialsError(LoginCommand command)
    {
        // given
        SetUpHappyPath(command);

        userRepository
            .Setup(r => r.ValidatePassword(It.IsAny<User>(), command.Password))
            .ReturnsAsync(false);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<InvalidCredentialsError>());
    }

    [TestCaseSource(typeof(LoginCommandTestCases), nameof(LoginCommandTestCases.ValidLoginCommands))]
    public async Task GivenValidCredentials_WhenHandle_ThenReturnsGeneratedTokens(LoginCommand command)
    {
        // given
        SetUpHappyPath(command);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.InstanceOf<TokenPair>());
            Assert.That(result.AsT0.AccessToken, Is.EqualTo(AccessToken));
            Assert.That(result.AsT0.RefreshToken, Is.EqualTo(RefreshToken));
        }
    }
}