using Games.Domain.Entities;
using Games.Domain.Enums;

namespace Games.Domain.Tests.Data;

public class PlaceTileGames : TheoryData<Game>
{
    public PlaceTileGames()
    {
        foreach (var game in GameTestCases.ValidGames().Where(g => g.TurnPhase == TurnPhase.PlaceTile))
        {
            Add(game);
        }
    }
}