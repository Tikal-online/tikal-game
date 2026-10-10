using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class LobbiesWithOneOwnerAndMultiplePlayers : TheoryData<Lobby>
{
    public LobbiesWithOneOwnerAndMultiplePlayers()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies().Where(l => l.Players.Count > 1 && l.Players.Count(p => p.IsOwner) == 1))
        {
            Add(lobby);
        }
    }
}