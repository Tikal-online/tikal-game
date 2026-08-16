using FluentValidation;
using Lobbies.Domain.Events;
using Shared.Domain.Entities;
using Shared.Domain.Enums;

namespace Lobbies.Domain.Entities;

public sealed class Lobby : Entity
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public required int MaxPlayers { get; set; }

    public ICollection<Player> Players { get; set; } = [];

    public bool InGame { get; set; }

    public bool IsEmpty => Players.Count == 0;

    public bool IsFull => Players.Count == MaxPlayers;

    public bool CanBeStarted => Players.Count >= 2 && Players.All(p => p.IsReady) && !InGame;

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
        AddDomainEvent(new PlayerLeftEvent(player));

        if (IsEmpty || Players.Any(p => p.IsOwner))
        {
            return;
        }

        var firstPlayer = Players.First();

        firstPlayer.IsOwner = true;
        AddDomainEvent(new PlayerUpdatedEvent(firstPlayer));
    }

    public void AddPlayer(Player player)
    {
        Players.Add(player);
        AddDomainEvent(new PlayerJoinedEvent(player));
    }

    public Player? GetPlayer(string userId)
    {
        return Players.FirstOrDefault(p => p.UserId == userId);
    }

    public Colour GetUnusedColour()
    {
        var usedColours = Players.Select(p => p.SelectedColour).ToHashSet();

        var unusedColour = Enum.GetValues<Colour>()
            .Except(usedColours)
            .First();

        return unusedColour;
    }

    public void Start()
    {
        InGame = true;
        AddDomainEvent(new LobbyStartedEvent(this));
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