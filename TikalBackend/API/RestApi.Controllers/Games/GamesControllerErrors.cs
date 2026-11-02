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
}