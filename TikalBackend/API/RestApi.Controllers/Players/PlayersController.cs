using Lobbies.Contracts.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Api;

namespace RestApi.Controllers.Players;

[RequireAccount]
public sealed partial class PlayersController : ApiController
{
    private readonly ISender sender;

    public PlayersController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPut("me/ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Changes the lobby status of the player of the currently authenticated user to ready")]
    public async Task<IActionResult> SetPlayerReady(CancellationToken cancellationToken)
    {
        var command = new SetPlayerReadyCommand();

        var result = await sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => Ok(),
            _ => PlayerNotInALobby(GetCurrentUserId())
        );
    }
}