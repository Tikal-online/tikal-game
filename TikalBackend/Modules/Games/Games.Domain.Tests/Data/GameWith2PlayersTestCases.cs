using Games.Domain.Entities;

namespace Games.Domain.Tests.Data;

public class GameWith2Players : TheoryData<Game>
{
    public GameWith2Players()
    {
        foreach (var game in GameTestCases.ValidGames().Where(g => g.Players.Count == 2))
        {
            Add(game);
        }
    }
}