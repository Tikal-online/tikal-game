using Games.Application.DataAccess;
using Games.Contracts.Commands;
using Games.Contracts.Errors;
using OneOf;
using OneOf.Types;

namespace Games.Application.UseCases.LeaveGame;

public sealed class LeaveGameCommandHandler
{
    private readonly PlayerRepository playerRepository;

    private readonly GameRepository gameRepository;

    public LeaveGameCommandHandler(
        PlayerRepository playerRepository,
        GameRepository gameRepository
    )
    {
        this.playerRepository = playerRepository;
        this.gameRepository = gameRepository;
    }

    public async Task<OneOf<Success, GameNotFound, PlayerNotInGivenGame>> Handle(
        LeaveGameCommand command,
        CancellationToken cancellationToken
    )
    {
        var game = await gameRepository.GetById(command.GameId);

        if (game is null)
        {
            return new GameNotFound(command.GameId);
        }

        var player = game.GetPlayer(command.UserId);

        if (player is null)
        {
            return new PlayerNotInGivenGame(command.GameId);
        }

        game.RemovePlayer(player);
        playerRepository.Delete(player);

        if (!game.HasEnoughPlayers)
        {
            gameRepository.Delete(game);
        }

        return new Success();
    }
}