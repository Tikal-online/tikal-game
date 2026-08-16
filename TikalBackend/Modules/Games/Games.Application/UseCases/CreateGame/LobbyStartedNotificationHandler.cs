using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Domain.Entities;
using Lobbies.Contracts.Notifications;
using MediatR;

namespace Games.Application.UseCases.CreateGame;

internal sealed class LobbyStartedNotificationHandler : INotificationHandler<LobbyStartedNotification>
{
    private readonly GameRepository gameRepository;

    private readonly UnitOfWork unitOfWork;

    public LobbyStartedNotificationHandler(GameRepository gameRepository, UnitOfWork unitOfWork)
    {
        this.gameRepository = gameRepository;
        this.unitOfWork = unitOfWork;
    }

    public Task Handle(LobbyStartedNotification notification, CancellationToken cancellationToken)
    {
        var players = PlayerMapper.DictionaryToPlayers(notification.Players);

        var game = new Game
        {
            LobbyId = notification.LobbyId,
            Players = players
        };

        gameRepository.Create(game);

        return unitOfWork.SaveChangesAsync(cancellationToken);
    }
}