using Games.Contracts.Models;
using Games.Contracts.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Games.Dtos;
using RestApi.Controllers.Games.Mappers;
using Shared.Api;
using Wolverine;

namespace RestApi.Controllers.Games;

[RequireAccount]
public sealed partial class GamesController : ApiController
{
    private readonly AccountContext accountContext;

    private readonly IMessageBus bus;

    public GamesController(IMessageBus bus, AccountContext accountContext)
    {
        this.bus = bus;
        this.accountContext = accountContext;
    }

    [HttpGet("me")]
    [ProducesResponseType<GameDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Gets the game for the currently authenticated user")]
    public async Task<IActionResult> GetGameForUser(CancellationToken cancellationToken)
    {
        var query = new GetGameForPlayerQuery(accountContext.Account.UserId);

        var result = await bus.InvokeAsync<GameModel?>(query, cancellationToken);

        if (result is null)
        {
            var userId = GetCurrentUserId();

            return PlayerNotInAGame(userId);
        }

        var gameDto = GameModelMapper.GameModelToGameDto(result);

        return Ok(gameDto);
    }
}