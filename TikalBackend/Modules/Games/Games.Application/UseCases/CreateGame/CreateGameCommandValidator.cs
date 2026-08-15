using FluentValidation;
using Games.Contracts.Commands;
using Games.Domain.Entities;

namespace Games.Application.UseCases.CreateGame;

public sealed class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(x => x.Players.Count)
            .ValidPlayerCount();
    }
}