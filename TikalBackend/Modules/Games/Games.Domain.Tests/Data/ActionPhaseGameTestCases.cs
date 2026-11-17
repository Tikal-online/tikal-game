using Games.Domain.Entities;
using Games.Domain.Enums;

namespace Games.Domain.Tests.Data;

public class ActionPhaseGames : TheoryData<Game>
{
    public ActionPhaseGames()
    {
        foreach (var game in GameTestCases.ValidGames().Where(g => g.TurnPhase == TurnPhase.Actions))
        {
            Add(game);
        }
    }
}