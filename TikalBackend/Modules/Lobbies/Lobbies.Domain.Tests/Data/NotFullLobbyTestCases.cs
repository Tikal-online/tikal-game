using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class NotFullLobbies : TheoryData<Lobby>
{
    public NotFullLobbies()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies().Where(l => !l.IsFull))
        {
            Add(lobby);
        }
    }
}