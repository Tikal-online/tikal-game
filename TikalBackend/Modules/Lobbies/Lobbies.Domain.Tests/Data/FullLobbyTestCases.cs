using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class FullLobbies : TheoryData<Lobby>
{
    public FullLobbies()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies().Where(l => l.IsFull))
        {
            Add(lobby);
        }
    }
}