using Lobbies.Domain.Entities;

namespace Lobbies.Application.DataAccess;

public interface LobbyRepository
{
    void Create(Lobby lobby);

    void Delete(Lobby lobby);
}