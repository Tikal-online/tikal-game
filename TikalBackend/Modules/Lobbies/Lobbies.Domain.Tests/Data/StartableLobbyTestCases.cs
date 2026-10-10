using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class StartableLobbies : TheoryData<Lobby>
{
    public StartableLobbies()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies().Where(l => l.CanBeStarted))
        {
            Add(lobby);
        }
    }
}