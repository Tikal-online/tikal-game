using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Entities;
using MediatR;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.JoinLobby;

internal sealed class JoinLobbyCommandHandler
    : CommandHandler<JoinLobbyCommand, OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull>>
{
    private readonly LobbyRepository lobbyRepository;

    private readonly PlayerQueryContext playerQueryContext;

    private readonly UnitOfWork unitOfWork;

    private readonly IPublisher publisher;

    private readonly AccountContext accountContext;

    public JoinLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        PlayerQueryContext playerQueryContext,
        UnitOfWork unitOfWork,
        IPublisher publisher,
        AccountContext accountContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.playerQueryContext = playerQueryContext;
        this.unitOfWork = unitOfWork;
        this.publisher = publisher;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull>> Handle(
        JoinLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(accountContext.Account.UserId);

        if (playerAlreadyInALobby)
        {
            return new PlayerAlreadyInALobby();
        }

        var lobby = await lobbyRepository.GetByIdAsync(request.LobbyId);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        if (lobby.IsFull)
        {
            return new LobbyFull(lobby.Id);
        }

        var colour = lobby.GetUnusedColour();

        var player = new Player
        {
            UserId = accountContext.Account.UserId,
            SelectedColour = colour,
            IsOwner = false,
            IsReady = false
        };

        lobby.Players.Add(player);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishPlayerJoinedNotification(player, cancellationToken);

        return new Success();
    }

    private Task PublishPlayerJoinedNotification(Player player, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(player, accountContext.Account);

        var notification = new PlayerJoinedNotification(playerModel, player.LobbyId);

        return publisher.Publish(notification, cancellationToken);
    }
}