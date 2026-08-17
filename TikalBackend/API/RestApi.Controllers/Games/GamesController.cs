using Games.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Games.Dtos;
using RestApi.Controllers.Games.Mappers;
using Shared.Api;

namespace RestApi.Controllers.Games;

[RequireAccount]
public sealed partial class GamesController : ApiController
{
    private readonly ISender sender;

    public GamesController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet("me")]
    [ProducesResponseType<GameDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Gets the game for the currently authenticated user")]
    public async Task<IActionResult> GetGameForUser(CancellationToken cancellationToken)
    {
        var query = new GetGameForAuthenticatedPlayerQuery();

        var result = await sender.Send(query, cancellationToken);

        if (result is null)
        {
            var userId = GetCurrentUserId();

            return PlayerNotInAGame(userId);
        }

        var gameDto = GameModelMapper.GameModelToGameDto(result);

        return Ok(gameDto);
    }
}