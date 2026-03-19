using OneOf;
using Shared.Contracts.Messaging;
using Users.Application.Services;
using Users.Contracts.Commands;
using Users.Contracts.Errors;
using Users.Contracts.Models;

namespace Users.Application.Usecases.Refresh;

internal sealed class RefreshCommandHandler : CommandHandler<RefreshCommand, OneOf<TokenPair, InvalidTokenError>>
{
    private readonly UserRepository userRepository;

    private readonly TokenService tokenService;

    public RefreshCommandHandler(UserRepository userRepository, TokenService tokenService)
    {
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    public async Task<OneOf<TokenPair, InvalidTokenError>> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        var tokenIsValid = await tokenService.ValidateToken(request.Token);

        if (!tokenIsValid)
        {
            return new InvalidTokenError();
        }

        var username = await tokenService.ExtractClaim<string>(request.Token, "name");

        if (username is null)
        {
            return new InvalidTokenError();
        }

        var user = await userRepository.GetByUsername(username);

        if (user is null)
        {
            return new InvalidTokenError();
        }

        var roles = await userRepository.GetRoles(user);

        var accessToken = tokenService.GenerateAccessToken(user, roles);
        var refreshToken = tokenService.GenerateRefreshToken(user, roles);

        return new TokenPair(accessToken, refreshToken);
    }
}