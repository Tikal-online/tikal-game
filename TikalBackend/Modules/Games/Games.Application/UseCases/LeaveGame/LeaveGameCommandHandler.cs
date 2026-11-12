using Games.Application.DataAccess;
using Games.Contracts.Commands;
using Games.Contracts.Errors;
using OneOf;
using OneOf.Types;

namespace Games.Application.UseCases.LeaveGame;

public sealed class LeaveGameCommandHandler
{
    private readonly GameRepository gameRepository;

    public LeaveGameCommandHandler(GameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<OneOf<Success, GameNotFound, PlayerNotInGivenGame>> Handle(
        LeaveGameCommand command,
        CancellationToken cancellationToken
    )
    {
        var game = await gameRepository.GetById(command.GameId, cancellationToken);

        if (game is null)
        {
            return new GameNotFound(command.GameId);
        }

        var result = game.RemovePlayer(command.UserId);

        return result.Match<OneOf<Success, GameNotFound, PlayerNotInGivenGame>>(
            success =>
            {
                if (!game.HasEnoughPlayers)
                {
                    gameRepository.Delete(game);
                }

                return success;
            },
            success => new PlayerNotInGivenGame(game.Id)
        );
    }
}