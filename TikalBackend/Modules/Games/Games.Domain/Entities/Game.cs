using FluentValidation;

namespace Games.Domain.Entities;

public sealed class Game
{
    public long Id { get; set; }

    public required long LobbyId { get; set; }

    public ICollection<Player> Players { get; set; } = [];

    public TileMap TileMap { get; set; } = null!;
}

public static class GameValidationRules
{
    extension<T>(IRuleBuilder<T, int> ruleBuilder)
    {
        public void ValidPlayerCount()
        {
            ruleBuilder
                .InclusiveBetween(2, 4)
                .WithMessage("Players must be between 2 and 4");
        }
    }
}