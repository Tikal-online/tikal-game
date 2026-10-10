using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class LobbiesWithMoreThanOnePlayer : TheoryData<Lobby>
{
    public LobbiesWithMoreThanOnePlayer()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies().Where(l => l.Players.Count > 1))
        {
            Add(lobby);
        }
    }
}