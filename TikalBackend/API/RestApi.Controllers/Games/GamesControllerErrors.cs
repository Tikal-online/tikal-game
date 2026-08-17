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
}