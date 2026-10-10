using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class ValidLobbies : TheoryData<Lobby>
{
    public ValidLobbies()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies())
        {
            Add(lobby);
        }
    }
}