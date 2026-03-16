using OneOf;
using Shared.Contracts.Messaging;
using Users.Application.Services;
using Users.Contracts.Commands;
using Users.Contracts.Errors;
using Users.Contracts.Models;

namespace Users.Application.Usecases.Login;

internal sealed class LoginCommandHandler : CommandHandler<LoginCommand, OneOf<TokenPair, InvalidCredentialsError>>
{
    private readonly UserRepository userRepository;

    private readonly TokenService tokenService;

    public LoginCommandHandler(UserRepository userRepository, TokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<OneOf<TokenPair, InvalidCredentialsError>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await userRepository.GetByUsername(request.Username);

        if (user is null)
        {
            return new InvalidCredentialsError();
        }

        var validPassword = await userRepository.ValidatePassword(user, request.Password);

        if (!validPassword)
        {
            return new InvalidCredentialsError();
        }

        var roles = await userRepository.GetRoles(user);

        var accessToken = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken(user, roles);

        return new TokenPair(accessToken, refreshToken);
    }
}