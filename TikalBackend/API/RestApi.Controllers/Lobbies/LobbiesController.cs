using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Lobbies.Dtos;
using RestApi.Controllers.Lobbies.Mappers;
using RestApi.Controllers.Shared;

namespace RestApi.Controllers.Lobbies;

public sealed partial class LobbiesController : ApiController
{
    private readonly ISender sender;

    public LobbiesController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    [ProducesResponseType<LobbyDto>(StatusCodes.Status201Created)]
    [EndpointDescription("Creates a new lobby containing the currently authenticated user.")]
    public async Task<IActionResult> CreateLobby(
        CreateLobbyDto createLobbyDto,
        CancellationToken cancellationToken
    )
    {
        var userId = GetCurrentUserId();

        var command = new CreateLobbyCommand(userId, createLobbyDto.Name, createLobbyDto.MaxPlayers);

        var result = await sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            lobbyModel => CreatedAtAction(nameof(CreateLobby), LobbyModelMapper.LobbyModelToLobbyDto(lobbyModel)),
            _ => MissingUserAccount(),
            _ => PlayerAlreadyInALobby()
        );
    }

    [HttpGet("{Id:long}")]
    [ProducesResponseType<List<LobbyDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Gets the lobby with the provided Id")]
    public async Task<IActionResult> GetLobby(long Id, CancellationToken cancellationToken)
    {
        var query = new GetLobbyQuery(Id);

        var result = await sender.Send(query, cancellationToken);

        if (result is null)
        {
            return LobbyNotFound(Id);
        }

        var lobbyDto = LobbyModelMapper.LobbyModelToLobbyDto(result);

        return Ok(lobbyDto);
    }
}