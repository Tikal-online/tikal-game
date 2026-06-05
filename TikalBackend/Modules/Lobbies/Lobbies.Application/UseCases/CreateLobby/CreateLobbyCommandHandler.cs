using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Enums;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Models;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Enums;
using OneOf;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.CreateLobby;

internal sealed class CreateLobbyCommandHandler
    : CommandHandler<CreateLobbyCommand, OneOf<LobbyModel, PlayerAlreadyInALobby>>
{
    private readonly LobbyRepository lobbyRepository;

    private readonly PlayerQueryContext playerQueryContext;

    private readonly UnitOfWork unitOfWork;

    private readonly AccountContext accountContext;

    public CreateLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        PlayerQueryContext playerQueryContext,
        UnitOfWork unitOfWork,
        AccountContext accountContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.playerQueryContext = playerQueryContext;
        this.unitOfWork = unitOfWork;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<LobbyModel, PlayerAlreadyInALobby>> Handle(
        CreateLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(accountContext.Account.UserId);

        if (playerAlreadyInALobby)
        {
            return new PlayerAlreadyInALobby();
        }

        var player = new Player
        {
            UserId = accountContext.Account.UserId,
            SelectedColour = Colour.Red,
            IsOwner = true,
            IsReady = false
        };

        var lobby = new Lobby
        {
            Name = request.Name,
            MaxPlayers = request.MaxPlayers,
            Players = [player]
        };

        lobbyRepository.Create(lobby);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var lobbyModel = LobbyMapper.LobbyToLobbyModel(lobby);

        lobbyModel.Players =
        [
            new LobbyPlayerModel
            {
                Name = accountContext.Account.Name,
                UserId = player.UserId,
                SelectedColour = (ColourModel)player.SelectedColour,
                IsOwner = player.IsOwner,
                IsReady = player.IsReady
            }
        ];

        return lobbyModel;
    }
}