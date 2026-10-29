using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using Shared.Api;
using Wolverine;

namespace RestApi.Controllers.Players;

[RequireAccount]
public sealed partial class PlayersController : ApiController
{
    private readonly IMessageBus bus;

    private readonly ISender sender;

    public PlayersController(ISender sender, IMessageBus bus)
    {
        this.sender = sender;
        this.bus = bus;
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

    [HttpDelete("me/ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Changes the lobby status of the player of the currently authenticated user to not ready")]
    public async Task<IActionResult> SetPlayerNotReady(CancellationToken cancellationToken)
    {
        var command = new SetPlayerNotReadyCommand();

        var result = await bus.InvokeAsync<OneOf<Success, PlayerNotInALobby>>(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => Ok(),
            _ => PlayerNotInALobby(GetCurrentUserId())
        );
    }
}