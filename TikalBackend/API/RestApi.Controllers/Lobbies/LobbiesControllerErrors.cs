using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers.Lobbies;

public sealed partial class LobbiesController
{
    private ObjectResult PlayerAlreadyInALobby()
    {
        return Problem(
            title: "Player is already in a lobby",
            detail: "A player may only be in one lobby at a time",
            statusCode: StatusCodes.Status409Conflict
        );
    }

    private ObjectResult PlayerNotInALobby(string userId)
    {
        return Problem(
            title: "Player is not in a lobby",
            detail: $"Player with userId {userId} is not in a lobby",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private ObjectResult LobbyNotFound(long id)
    {
        return Problem(
            title: "Lobby not found",
            detail: $"Lobby with ID {id} was not found",
            statusCode: StatusCodes.Status404NotFound
        );
    }
}