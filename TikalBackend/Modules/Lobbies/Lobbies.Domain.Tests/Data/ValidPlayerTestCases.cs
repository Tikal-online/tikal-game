using Lobbies.Domain.Entities;

namespace Lobbies.Domain.Tests.Data;

public class ValidPlayers : TheoryData<Player>
{
    public ValidPlayers()
    {
        foreach (var lobby in LobbyTestCases.ValidLobbies())
        {
            foreach (var player in lobby.Players)
            {
                Add(player);
            }
        }
    }
}