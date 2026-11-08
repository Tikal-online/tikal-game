using FluentValidation;
using Lobbies.Domain.Errors;
using Lobbies.Domain.Events;
using Lobbies.Domain.Mappers;
using OneOf;
using OneOf.Types;
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

    public uint Version { get; set; }

    public bool IsEmpty => Players.Count == 0;

    public bool IsFull => Players.Count == MaxPlayers;

    public bool CanBeStarted => Players.Count >= 2 && Players.All(p => p.IsReady) && !InGame;

    public OneOf<Success, PlayerNotInLobbyDomainError, LobbyInGameDomainError> RemovePlayer(string userId)
    {
        if (InGame)
        {
            return new LobbyInGameDomainError();
        }

        var player = Players.FirstOrDefault(p => p.UserId == userId);

        if (player is null)
        {
            return new PlayerNotInLobbyDomainError();
        }

        Players.Remove(player);
        Touch();
        AddDomainEvent(new PlayerLeftDomainEvent(PlayerMapper.PlayerToPlayerEventData(player), Id));

        // If there is no owner left, promote another random player to owner
        if (IsEmpty || Players.Any(p => p.IsOwner))
        {
            return new Success();
        }

        var firstPlayer = Players.First();

        firstPlayer.IsOwner = true;
        AddDomainEvent(new PlayerUpdatedDomainEvent(PlayerMapper.PlayerToPlayerEventData(firstPlayer), Id));

        return new Success();
    }

    public OneOf<Success, LobbyInGameDomainError, LobbyFullDomainError> AddPlayer(string username, string userId)
    {
        if (InGame)
        {
            return new LobbyInGameDomainError();
        }

        if (IsFull)
        {
            return new LobbyFullDomainError();
        }

        var colour = GetUnusedColour();

        var player = new Player
        {
            UserId = userId,
            Name = username,
            SelectedColour = colour,
            IsOwner = false,
            IsReady = false
        };

        Players.Add(player);
        Touch();
        AddDomainEvent(new PlayerJoinedDomainEvent(PlayerMapper.PlayerToPlayerEventData(player), Id));

        return new Success();
    }

    public OneOf<Success, LobbyCannotBeStartedDomainError> Start()
    {
        if (!CanBeStarted)
        {
            return new LobbyCannotBeStartedDomainError();
        }

        InGame = true;
        AddDomainEvent(new LobbyStartedDomainEvent(LobbyMapper.LobbyToLobbyEventData(this)));

        return new Success();
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

    public void Stop()
    {
        foreach (var player in Players)
        {
            player.ReadyDown();
        }

        InGame = false;
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