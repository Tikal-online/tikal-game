using FluentValidation;
using Lobbies.Contracts.Commands;
using Lobbies.Domain.Entities;

namespace Lobbies.Application.UseCases.CreateLobby;

public sealed class CreateLobbyCommandValidator : AbstractValidator<CreateLobbyCommand>
{
    public CreateLobbyCommandValidator()
    {
        RuleFor(x => x.UserId)
            .ValidPlayerUserId();

        RuleFor(x => x.Name)
            .ValidLobbyName();

        RuleFor(x => x.MaxPlayers)
            .ValidMaxPlayers();
    }
}