using FluentValidation;

namespace Lobbies.Domain.Entities;

public sealed class Lobby
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public required int MaxPlayers { get; set; }

    public ICollection<Player> Players { get; set; } = [];

    public bool IsEmpty => Players.Count == 0;

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);

        if (Players.Count == 0 || Players.Any(p => p.IsOwner))
        {
            return;
        }

        Players.First().IsOwner = true;
    }
}

public static class LobbyValidationRules
{
    extension<T>(IRuleBuilder<T, string> ruleBuilder)
    {
        public void ValidLobbyName()
        {
            ruleBuilder
                .NotEmpty()
                .WithMessage("Name cannot be empty")
                .MaximumLength(30)
                .WithMessage("Name cannot exceed 30 characters");
        }
    }

    extension<T>(IRuleBuilder<T, int> ruleBuilder)
    {
        public void ValidMaxPlayers()
        {
            ruleBuilder
                .InclusiveBetween(2, 4)
                .WithMessage("MaxPlayers must be between 2 and 4");
        }
    }
}