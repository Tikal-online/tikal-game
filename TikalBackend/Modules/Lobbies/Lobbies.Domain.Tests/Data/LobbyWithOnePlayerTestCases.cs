using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class LobbiesWithOnePlayer : TheoryData<Lobby>
{
    public LobbiesWithOnePlayer()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies().Where(l => l.Players.Count == 1))
        {
            Add(lobby);
        }
    }
}