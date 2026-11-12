using Games.Contracts.Events;
using Lobbies.Application.DataAccess;

namespace Lobbies.Application.UseCases.LeaveLobby;

public sealed class GamePlayerLeftIntegrationEventHandler
{
    private readonly LobbyRepository lobbyRepository;

    public GamePlayerLeftIntegrationEventHandler(LobbyRepository lobbyRepository)
    {
        this.lobbyRepository = lobbyRepository;
    }

    public async Task Handle(PlayerLeftIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var lobby = await lobbyRepository.GetById(integrationEvent.LobbyId, cancellationToken);

        lobby?.RemoveInGamePlayer(integrationEvent.GamePlayerModel.UserId);
    }
}