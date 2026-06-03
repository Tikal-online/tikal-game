using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Models;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Enums;
using MediatR;
using OneOf;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.CreateLobby;

internal sealed class CreateLobbyCommandHandler
    : CommandHandler<CreateLobbyCommand, OneOf<LobbyModel, MissingUserAccount, PlayerAlreadyInALobby>>
{
    private readonly LobbyRepository lobbyRepository;

    private readonly PlayerQueryContext playerQueryContext;

    private readonly ISender sender;

    private readonly UnitOfWork unitOfWork;

    public CreateLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        PlayerQueryContext playerQueryContext,
        ISender sender,
        UnitOfWork unitOfWork
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.playerQueryContext = playerQueryContext;
        this.sender = sender;
        this.unitOfWork = unitOfWork;
    }

    public async Task<OneOf<LobbyModel, MissingUserAccount, PlayerAlreadyInALobby>> Handle(
        CreateLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var account = await sender.Send(new GetAccountQuery(request.UserId), cancellationToken);

        if (account is null)
        {
            return new MissingUserAccount();
        }

        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(account.UserId);

        if (playerAlreadyInALobby)
        {
            return new PlayerAlreadyInALobby();
        }

        var player = new Player
        {
            UserId = account.UserId,
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

        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(player, account);

        var lobbyModel = LobbyMapper.LobbyToLobbyModel(lobby);

        lobbyModel.Players = [playerModel];

        return lobbyModel;
    }
}