using FluentValidation;
using Lobbies.Domain.Enums;

namespace Lobbies.Domain.Entities;

public sealed class Player
{
    public long Id { get; set; }

    public required string UserId { get; set; }

    public required Colour SelectedColour { get; set; }

    public bool IsReady { get; set; }

    public bool IsOwner { get; set; }

    public long LobbyId { get; set; }

    public Lobby Lobby { get; set; } = null!;
}

public static class AccountValidationRules
{
    extension<T>(IRuleBuilder<T, string> ruleBuilder)
    {
        public void ValidPlayerUserId()
        {
            ruleBuilder
                .NotEmpty()
                .WithMessage("UserId cannot be empty")
                .MaximumLength(100)
                .WithMessage("UserId cannot exceed 100 characters");
        }
    }
}