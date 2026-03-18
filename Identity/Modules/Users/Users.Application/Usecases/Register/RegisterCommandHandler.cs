using OneOf;
using OneOf.Types;
using Shared.Contracts.Messaging;
using Users.Contracts.Commands;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Application.Usecases.Register;

internal sealed class RegisterCommandHandler : CommandHandler<RegisterCommand, OneOf<Success, DuplicateUsernameError>>
{
    private readonly UserRepository userRepository;

    public RegisterCommandHandler(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<OneOf<Success, DuplicateUsernameError>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await userRepository.GetByUsername(request.Username);

        if (existingUser is not null)
        {
            return new DuplicateUsernameError(request.Username);
        }

        var newUser = new User { UserName = request.Username };

        var userCreationResult = await userRepository.CreateUser(newUser, request.Password);

        if (!userCreationResult.TryPickT0(out var user, out var error))
        {
            return error;
        }

        await userRepository.AssignRole(user, "User");

        return new Success();
    }
}