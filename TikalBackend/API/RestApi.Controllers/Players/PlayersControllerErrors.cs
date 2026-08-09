using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Players;

public sealed partial class PlayersController
{
    private ObjectResult PlayerNotInALobby(string userId)
    {
        return Problem(
            title: "Player is not in a lobby",
            detail: $"Player with userId {userId} is not in a lobby",
            statusCode: StatusCodes.Status404NotFound
        );
    }
}