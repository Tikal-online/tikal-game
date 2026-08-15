using Games.Contracts.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Api;

namespace RestApi.Controllers.Games;

[RequireAccount]
public sealed class GamesController : ApiController
{
    private readonly ISender sender;

    public GamesController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame(CancellationToken cancellationToken)
    {
        var command = new CreateGameCommand();

        await sender.Send(command, cancellationToken);

        return Ok();
    }
}