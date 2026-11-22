using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Games;

public sealed partial class GamesController
{
    private ObjectResult PlayerNotInAGame(string userId)
    {
        return Problem(
            title: "Player is not in a game",
            detail: $"Player with userId {userId} is not in a game",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private ObjectResult PlayerNotInGivenGame(long gameId)
    {
        return Problem(
            title: "Player is not in the given game",
            detail: $"Player is not in the game with ID {gameId}",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private ObjectResult GameNotFound(long gameId)
    {
        return Problem(
            title: "Game not found",
            detail: $"Game with ID {gameId} was not found",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private ObjectResult NoTileLeft(long gameId)
    {
        return Problem(
            title: "No tile left",
            detail: $"Game with ID {gameId} has not tile left",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private ObjectResult InvalidTurnPhase(long gameId)
    {
        return Problem(
            title: "Invalid turn phase",
            detail: $"Game with ID {gameId} is not in a valid turn phase to perform this action",
            statusCode: StatusCodes.Status409Conflict
        );
    }

    private ObjectResult TileMustBeDrawnFirst(long gameId)
    {
        return Problem(
            title: "Tile must be draw first",
            detail: $"The game with ID {gameId} must first have a drawn tile before it can be placed",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private ObjectResult TileCannotBePlaced()
    {
        return Problem(
            title: "Tile can not be placed",
            detail: $"Invalid tile location provided",
            statusCode: StatusCodes.Status409Conflict
        );
    }
}
