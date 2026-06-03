using Lobbies.Contracts.Commands;
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
    [EndpointDescription("Creates a new lobby with the currently authenticated user.")]
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
}