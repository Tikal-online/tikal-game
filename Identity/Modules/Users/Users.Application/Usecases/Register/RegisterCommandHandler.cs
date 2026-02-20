using OneOf;
using Shared.Contracts.Messaging;
using Users.Contracts.Commands;
using Users.Contracts.Dtos;
using Users.Contracts.Errors;
using Users.Domain.Entities;

namespace Users.Application.Usecases.Register;

public class RegisterCommandHandler : CommandHandler<RegisterCommand, OneOf<UserDto, DuplicateUsernameError>>
{
    private readonly UserRepository userRepository;

    public RegisterCommandHandler(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<OneOf<UserDto, DuplicateUsernameError>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await userRepository.GetByUsername(request.Username, cancellationToken);

        if (existingUser is not null)
        {
            return new DuplicateUsernameError(request.Username);
        }

        var newUser = new User(request.Username);

        var result = await userRepository.CreateUser(newUser, request.Password, cancellationToken);

        return result.Match<OneOf<UserDto, DuplicateUsernameError>>(
            createdUser => new UserDto { Username = createdUser.Username },
            duplicateUsernameError => duplicateUsernameError
        );
    }
}