using Moq;
using Users.Application.Services;
using Users.Application.Usecases.Refresh;
using Users.Contracts.Commands;
using Users.Contracts.Errors;
using Users.Contracts.Models;
using Users.Domain.Entities;

namespace Users.Application.Tests.Usecases.Refresh;

internal sealed class RefreshCommandHandlerTests
{
    private const string AccessToken = "accessToken";
    private const string RefreshToken = "refreshToken";

    // dependencies
    private Mock<UserRepository> userRepository;
    private Mock<TokenService> tokenService;

    // under test
    private RefreshCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        userRepository = new Mock<UserRepository>();
        tokenService = new Mock<TokenService>();

        handler = new RefreshCommandHandler(userRepository.Object, tokenService.Object);
    }

    private void SetUpHappyPath(RefreshCommand command)
    {
        // token is valid
        tokenService
            .Setup(t => t.ValidateToken(command.Token))
            .ReturnsAsync(true);

        // token contains name claim
        const string username = "username";

        tokenService
            .Setup(t => t.ExtractClaim<string>(command.Token, "name"))
            .ReturnsAsync(username);

        // user with extracted name exists
        var user = new User { Id = 1, UserName = username };

        userRepository
            .Setup(r => r.GetByUsername(username))
            .ReturnsAsync(user);

        // new tokens are generated
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

    [TestCaseSource(typeof(RefreshCommandTestCases), nameof(RefreshCommandTestCases.ValidRefreshCommands))]
    public async Task GivenInvalidToken_WhenHandle_ThenReturnsInvalidTokenError(RefreshCommand command)
    {
        // given
        SetUpHappyPath(command);

        tokenService
            .Setup(t => t.ValidateToken(command.Token))
            .ReturnsAsync(false);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<InvalidTokenError>());
    }

    [TestCaseSource(typeof(RefreshCommandTestCases), nameof(RefreshCommandTestCases.ValidRefreshCommands))]
    public async Task GivenNonExistentNameClaim_WhenHandle_ThenReturnsInvalidTokenError(RefreshCommand command)
    {
        // given
        SetUpHappyPath(command);

        tokenService
            .Setup(t => t.ExtractClaim<string>(command.Token, "name"))
            .ReturnsAsync(default(string));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<InvalidTokenError>());
    }

    [TestCaseSource(typeof(RefreshCommandTestCases), nameof(RefreshCommandTestCases.ValidRefreshCommands))]
    public async Task GivenNoUserWithExtractedUsername_WhenHandle_ThenReturnsInvalidTokenError(RefreshCommand command)
    {
        // given
        SetUpHappyPath(command);

        userRepository
            .Setup(r => r.GetByUsername(It.IsAny<string>()))
            .ReturnsAsync(default(User));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.That(result.Value, Is.InstanceOf<InvalidTokenError>());
    }

    [TestCaseSource(typeof(RefreshCommandTestCases), nameof(RefreshCommandTestCases.ValidRefreshCommands))]
    public async Task GivenValidToken_WhenHandle_ThenReturnsGeneratedTokens(RefreshCommand command)
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