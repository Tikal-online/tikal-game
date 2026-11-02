using Games.Domain.Entities;

namespace Games.Domain.Tests.Data;

public class ValidGames : TheoryData<Game>
{
    public ValidGames()
    {
        foreach (var game in GameTestCases.ValidGames())
        {
            Add(game);
        }
    }
}