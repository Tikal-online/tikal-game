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

namespace Lobbies.Application.UseCases.LeaveLobby;

internal sealed class LeaveLobbyCommandHandler : CommandHandler<LeaveLobbyCommand, OneOf<Success, PlayerNotInALobby>>
{
    private readonly PlayerRepository playerRepository;

    private readonly LobbyRepository lobbyRepository;

    private readonly UnitOfWork unitOfWork;

    private readonly IPublisher publisher;

    private readonly AccountContext accountContext;

    public LeaveLobbyCommandHandler(
        PlayerRepository playerRepository,
        LobbyRepository lobbyRepository,
        UnitOfWork unitOfWork,
        IPublisher publisher,
        AccountContext accountContext
    )
    {
        this.playerRepository = playerRepository;
        this.lobbyRepository = lobbyRepository;
        this.unitOfWork = unitOfWork;
        this.publisher = publisher;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, PlayerNotInALobby>> Handle(
        LeaveLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var player = await playerRepository.GetByUserIdWithLobbyAsync(accountContext.Account.UserId);

        if (player is null)
        {
            return new PlayerNotInALobby();
        }

        player.LeaveLobby();
        playerRepository.Delete(player);

        if (player.Lobby.IsEmpty)
        {
            lobbyRepository.Delete(player.Lobby);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await PublishPlayerLeftNotification(player, cancellationToken);

        return new Success();
    }

    private Task PublishPlayerLeftNotification(Player player, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(player, accountContext.Account);

        var notification = new PlayerLeftNotification(playerModel, player.LobbyId);

        return publisher.Publish(notification, cancellationToken);
    }
}