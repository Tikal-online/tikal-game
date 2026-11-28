using Games.Application.DataAccess;
using Games.Contracts.Commands;
using Games.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Contracts.Errors;

namespace Games.Application.UseCases.EndTurn;

public sealed class EndTurnCommandHandler
{
    private readonly GameRepository gameRepository;

    public EndTurnCommandHandler(GameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<OneOf<Success, GameNotFound, Unprivileged, InvalidTurnPhase>> Handle(
        EndTurnCommand command,
        CancellationToken cancellationToken
    )
    {
        var game = await gameRepository.GetById(command.GameId, cancellationToken);

        if (game is null)
        {
            return new GameNotFound(command.GameId);
        }

        if (game.CurrentPlayer.UserId != command.UserId)
        {
            return new Unprivileged();
        }

        var result = game.EndTurn();

        return result.Match<OneOf<Success, GameNotFound, Unprivileged, InvalidTurnPhase>>(
            success => success,
            InvalidTurnPhase => new InvalidTurnPhase(game.Id)
        );
    }
}