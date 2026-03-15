using OneOf;
using OneOf.Types;
using Shared.Contracts.Messaging;
using Users.Contracts.Commands;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Application.Usecases.Register;

public class RegisterCommandHandler : CommandHandler<RegisterCommand, OneOf<Success, DuplicateUsernameError>>
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
        var existingUser = await userRepository.GetByUsername(request.Username, cancellationToken);

        if (existingUser is not null)
        {
            return new DuplicateUsernameError(request.Username);
        }

        var newUser = new User { Name = request.Username };

        var userRole = new Role { Name = "User" };

        newUser.Roles.Add(userRole);

        var result = await userRepository.CreateUser(newUser, request.Password, cancellationToken);

        return result.Match<OneOf<Success, DuplicateUsernameError>>(
            _ => new Success(),
            duplicateUsernameError => duplicateUsernameError
        );
    }
}