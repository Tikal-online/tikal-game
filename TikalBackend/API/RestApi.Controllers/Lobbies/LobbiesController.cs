using System.ComponentModel.DataAnnotations;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Controllers.Lobbies.Dtos;
using RestApi.Controllers.Lobbies.Mappers;
using RestApi.Controllers.Shared.Dtos;
using Shared.Api;

namespace RestApi.Controllers.Lobbies;

[RequireAccount]
public sealed partial class LobbiesController : ApiController
{
    private readonly ISender sender;

    public LobbiesController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointDescription("Creates a new lobby containing the currently authenticated user")]
    public async Task<IActionResult> CreateLobby(
        CreateLobbyDto createLobbyDto,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateLobbyCommand(createLobbyDto.Name, createLobbyDto.MaxPlayers);

        var result = await sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => CreatedAtAction(nameof(GetLobbyForUser), new { }),
            _ => PlayerAlreadyInALobby()
        );
    }

    [HttpGet("{Id:long}")]
    [ProducesResponseType<LobbyDto>(StatusCodes.Status200OK)]
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

    [HttpPost("{Id:long}/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [EndpointDescription("Joins the lobby for the currently authenticated user")]
    public async Task<IActionResult> JoinLobby(long Id, CancellationToken cancellationToken)
    {
        var query = new JoinLobbyCommand(Id);

        var result = await sender.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            _ => Ok(),
            _ => PlayerAlreadyInALobby(),
            lobbyNotFound => LobbyNotFound(lobbyNotFound.LobbyId),
            lobbyFull => LobbyFull(lobbyFull.LobbyId)
        );
    }

    [HttpGet("me")]
    [ProducesResponseType<LobbyDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Gets the lobby for the currently authenticated user")]
    public async Task<IActionResult> GetLobbyForUser(CancellationToken cancellationToken)
    {
        var query = new GetLobbyForAuthenticatedPlayerQuery();

        var result = await sender.Send(query, cancellationToken);

        if (result is null)
        {
            var userId = GetCurrentUserId();

            return PlayerNotInALobby(userId);
        }

        var lobbyDto = LobbyModelMapper.LobbyModelToLobbyDto(result);

        return Ok(lobbyDto);
    }

    [HttpPost("leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Leaves the lobby for the currently authenticated user")]
    public async Task<IActionResult> LeaveLobby(CancellationToken cancellationToken)
    {
        var command = new LeaveLobbyCommand();

        var result = await sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => Ok(),
            _ => PlayerNotInALobby(GetCurrentUserId())
        );
    }

    [HttpPost("sendMessage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointDescription("Sends a message to the lobby for the currently authenticated user")]
    public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto, CancellationToken cancellationToken)
    {
        var command = new SendLobbyChatMessageCommand(sendMessageDto.Message);

        var result = await sender.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            _ => Ok(),
            _ => PlayerNotInALobby(GetCurrentUserId())
        );
    }

    [HttpGet]
    [ProducesResponseType<PaginatedDto<List<LobbySummaryDto>>>(StatusCodes.Status200OK)]
    [EndpointDescription("Gets a paginated summary of the currently active lobbies. Can be filtered by lobby name")]
    public async Task<IActionResult> GetPaginatedLobbies(
        [FromQuery][Required][Range(1, int.MaxValue)] int pageSize,
        [FromQuery][Required][Range(1, int.MaxValue)] int pageNumber,
        [FromQuery] string? searchText,
        CancellationToken cancellationToken
    )
    {
        var query = new GetPaginatedLobbiesQuery(pageSize, pageNumber, searchText);

        var paginatedResult = await sender.Send(query, cancellationToken);

        var lobbySummaryDtos = LobbyModelMapper.LobbySummaryModelsToLobbySummaryDtos(paginatedResult.Data);

        var paginatedDto = new PaginatedDto<List<LobbySummaryDto>>
        {
            Data = lobbySummaryDtos,
            TotalCount = paginatedResult.TotalCount
        };

        return Ok(paginatedDto);
    }
}