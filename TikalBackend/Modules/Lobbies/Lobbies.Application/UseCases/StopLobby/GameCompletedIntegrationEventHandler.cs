using Games.Contracts.Events;
using Lobbies.Application.DataAccess;

namespace Lobbies.Application.UseCases.StopLobby;

public sealed class GameCompletedIntegrationEventHandler
{
    private readonly LobbyRepository lobbyRepository;

    public GameCompletedIntegrationEventHandler(LobbyRepository lobbyRepository)
    {
        this.lobbyRepository = lobbyRepository;
    }

    public async Task Handle(GameCompletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var lobby = await lobbyRepository.GetById(integrationEvent.Game.LobbyId);

        lobby?.Stop();
    }
}