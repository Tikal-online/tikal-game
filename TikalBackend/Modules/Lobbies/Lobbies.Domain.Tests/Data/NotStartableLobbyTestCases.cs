using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class NotStartableLobbies : TheoryData<Lobby>
{
    public NotStartableLobbies()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies().Where(l => !l.CanBeStarted))
        {
            Add(lobby);
        }
    }
}